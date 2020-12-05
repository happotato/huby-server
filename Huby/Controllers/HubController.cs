using System;
using System.Linq;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Huby.Data;
using Huby.Data.Extensions;

namespace Huby.Controllers
{
    public sealed class HubCreateData
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; }

        [Required]
        public bool IsNSFW { get; set; }

        [Required]
        public int BannerColor {get; set;}
    }

    public sealed class TopicCreateData
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public ContentType ContentType { get; set; }

        [Required]
        public bool IsNSFW { get; set; }

        public string Tags { get; set; }
    }

    public sealed class HubQueryResult
    {
        public Permissions Permissions { get; set; }
        public bool Subscribed { get; set; }
        public Hub Hub { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class HubController : ControllerBase
    {
        private ILogger<HubController> Logger { get; }
        private ApplicationDatabase Database  { get; }

        public HubController(ILogger<HubController> logger, ApplicationDatabase db)
        {
            Logger = logger;
            Database = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Hub>> Get(
            string name = null,
            string owner = null,
            int page = 0,
            int limit = 10,
            string after = null)
        {
            return Database.Hubs
                .Include(hub => hub.Owner)
                .Where(hub => name == null || EF.Functions.ILike(hub.Name, $"%{name}%"))
                .Where(hub => owner == null || hub.Owner.Username == owner)
                .OrderBy(hub=> hub.CreatedAt)
                .Paginate(page, limit, after)
                .ToList();
        }

        [HttpGet("{name}")]
        public ActionResult<HubQueryResult> GetHubByName(string name)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.GetUserByUsername(currentUsername);

            var hub = Database.Hubs
                .Include(hub => hub.Owner)
                .Include(hub => hub.Subscriptions)
                .Include(hub => hub.Moderators)
                .Where(hub => hub.Name == name)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (hub == null)
                return NotFound($"Hub \"{name}\" not found");

            return new HubQueryResult
            {
                Permissions = hub.GetPermissions(currentUser?.Id),
                Subscribed = hub.IsSubscribed(currentUser?.Id),
                Hub = hub,
            };
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Hub> CreateHub(HubCreateData data)
        {
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = Database.GetUserByUsername(username);

            if (data.Name.ContainsSpecialCharacter())
                return BadRequest("Name cannot contain special characters");

            var hub = new Hub
            {
                Id = DbItem.GenerateID(),
                Name = data.Name,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                BannerColor = data.BannerColor,
                IsNSFW = data.IsNSFW,
                OwnerId = user.Id,
                Owner = user,
            };

            if (Database.Hubs.Any(h => h.Name == hub.Name))
                return Conflict("Hub already exists");

            Database.Hubs.Add(hub);
            Database.SaveChanges();

            return Created($"/api/hub/{hub.Name}", hub);
        }

        [Authorize]
        [HttpPatch("{name}")]
        public ActionResult<Hub> Patch(string name, JsonElement json)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.GetUserByUsername(currentUsername);

            var hub = Database.Hubs
                .Include(hub => hub.Owner)
                .Include(hub => hub.Subscriptions)
                .Include(hub => hub.Moderators)
                .Where(hub => hub.Name == name)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (hub == null)
                return NotFound($"Hub \"{name}\" not found");

            if (!hub.CanEdit(currentUser.Id))
                return Unauthorized();

            if (json.TryGetProperty("description", out JsonElement descriptionProperty))
            {
                if (descriptionProperty.ValueKind == JsonValueKind.String)
                {
                    var description = descriptionProperty.GetString();

                    if (description.Length == 0)
                        return BadRequest("Empty description");

                    hub.Description = description;
                }
                else
                {
                    return BadRequest("Property \"description\" must be string");
                }
            }

            if (json.TryGetProperty("imageUrl", out JsonElement imageUrlProperty))
            {
                if (imageUrlProperty.ValueKind == JsonValueKind.String)
                {
                    var imageUrl = imageUrlProperty.GetString();
                    var urlAttr = new UrlAttribute();

                    if (!urlAttr.IsValid(imageUrl))
                        return BadRequest("Invalid image url");

                    hub.ImageUrl = imageUrl;
                }
                else
                {
                    return BadRequest("Property \"imageUrl\" must be string");
                }
            }

            if (json.TryGetProperty("bannerColor", out JsonElement bannerColorProperty))
            {
                if (bannerColorProperty.ValueKind == JsonValueKind.Number)
                {
                    hub.BannerColor = bannerColorProperty.GetInt32();
                }
                else
                {
                    return BadRequest("Property \"bannerColor\" must be number");
                }
            }

            Database.SaveChanges();
            return hub;
        }

        [Authorize]
        [HttpDelete("{name}")]
        public ActionResult DeleteHub(string name)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.GetUserByUsername(currentUsername);

            var hub = Database.Hubs
                .Include(hub => hub.Moderators)
                .Where(hub => hub.Name == name)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (hub == null)
                return NotFound($"hub \"{name}\" not found");

            if (hub.OwnerId != currentUser.Id)
                return Unauthorized();

            Database.Remove(hub);
            Database.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost("{name}/topics")]
        public ActionResult<Topic> CreateTopic(string name, TopicCreateData data)
        {
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = Database.GetUserByUsername(username);

            var hub = Database.Hubs
                .Include(hub => hub.Owner)
                .Where(h => h.Name == name)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (hub == null)
                return NotFound($"Hub \"{name}\" not found");

            var topic = new Topic
            {
                Id = DbItem.GenerateID(),
                Title = data.Title,
                Content = data.Content,
                IsNSFW = hub.IsNSFW || data.IsNSFW,
                ContentType = data.ContentType,
                Tags = data.Tags ?? "",
                HubId = hub.Id,
                Hub = hub,
                OwnerId = user.Id,
                Owner = user,
            };

            Database.Posts.Add(topic);
            Database.SaveChanges();

            return Created($"/api/post/{topic.Id}", topic);
        }

        [Authorize]
        [HttpPost("{name}/subscribe")]
        public ActionResult<Subscription> Subscribe(string name)
        {
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = Database.GetUserByUsername(username);

            var hub = Database.Hubs
                .Include(hub => hub.Owner)
                .Where(h => h.Name == name)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (hub == null)
                return NotFound($"Hub \"{name}\" not found");

            {
                var exisingSub = Database.Subscriptions
                    .Where(sub => sub.UserId == user.Id)
                    .Where(sub => sub.HubId == hub.Id)
                    .ToList()
                    .DefaultIfEmpty(null)
                    .SingleOrDefault();

                if (exisingSub != null)
                    return exisingSub;
            }

            var sub = new Subscription
            {
                Id = DbItem.GenerateID(),
                UserId = user.Id,
                HubId = hub.Id,
                Hub = hub,
                User = user,
            };

            hub.SubscribersCount += 1;

            Database.Subscriptions.Add(sub);
            Database.SaveChanges();

            return sub;
        }

        [Authorize]
        [HttpDelete("{name}/unsubscribe")]
        public ActionResult Unsubscribe(string name)
        {
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = Database.GetUserByUsername(username);

            var sub = Database.Subscriptions
                .Include(sub => sub.User)
                .Include(sub => sub.Hub)
                .Where(sub => sub.User.Id == user.Id)
                .Where(sub => sub.Hub.Name == name)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (sub == null)
                return null;

            sub.Hub.SubscribersCount -= 1;

            Database.Subscriptions.Remove(sub);
            Database.SaveChanges();

            return Ok();
        }
    }
}
