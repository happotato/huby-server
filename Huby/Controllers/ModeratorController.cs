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
    public sealed class ModeratorCreateData
    {
        [Required]
        public string Hub { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public Permissions Permissions { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ModeratorController : ControllerBase
    {
        private ILogger<ModeratorController> Logger { get; }
        private ApplicationDatabase Database        { get; }

        public ModeratorController(ILogger<ModeratorController> logger, ApplicationDatabase db)
        {
            Logger = logger;
            Database = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Moderator>> GetModerators(
            string hub = null,
            string username = null,
            int page = 0,
            int limit = 10,
            string after = null)
        {
            return Database.Moderators
                .Include(mod => mod.User)
                .Include(mod => mod.Hub.Owner)
                .Where(mod => hub == null || mod.Hub.Name.Contains(hub))
                .Where(mod => username == null || mod.Hub.Owner.Username == username)
                .OrderBy(hub=> hub.CreatedAt)
                .Paginate(page, limit, after)
                .ToList();
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Moderator> CreateModerator(ModeratorCreateData data)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.GetUserByUsername(currentUsername);

            var hub = Database.Hubs
                .Include(hub => hub.Owner)
                .Include(hub => hub.Moderators)
                .Where(hub => hub.Name == data.Hub)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (hub == null)
                return NotFound($"Hub \"{data.Hub}\" not found");

            if (!hub.CanEdit(currentUser.Id))
                return Unauthorized();

            var user = Database.Users
                .Where(user => user.Username == data.Username)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (user == null)
                return NotFound($"User \"{data.Username}\" not found");

            if (hub.Moderators.Any(mod => mod.UserId == user.Id))
                return Conflict();

            var mod = new Moderator
            {
                Id = DbItem.GenerateID(),
                HubId = hub.Id,
                Hub = hub,
                UserId = user.Id,
                User = user,
                CanEdit = data.Permissions.CanEdit,
                CanDeletePosts = data.Permissions.CanDeletePosts,
            };

            Database.Moderators.Add(mod);
            Database.SaveChanges();

            return Created($"/api/moderator/{mod.Id}", mod);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public ActionResult<Moderator> PatchModerator(string id, Permissions permissions)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.GetUserByUsername(currentUsername);

            var mod = Database.Moderators
                .Include(mod => mod.User)
                .Include(mod => mod.Hub.Moderators)
                .Where(mod => mod.Id == id)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (mod == null)
                return NotFound($"mod \"{id}\" not found");

            if (!mod.Hub.CanEdit(currentUser.Id))
                return Unauthorized();

            mod.CanEdit = permissions.CanEdit;
            mod.CanDeletePosts = permissions.CanDeletePosts;

            Database.SaveChanges();

            return Ok(mod);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteModerator(string id)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.GetUserByUsername(currentUsername);

            var mod = Database.Moderators
                .Include(mod => mod.Hub.Moderators)
                .Where(mod => mod.Id == id)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (mod == null)
                return NotFound($"mod \"{id}\" not found");

            if (!mod.Hub.CanEdit(currentUser.Id))
                return Unauthorized();

            Database.Remove(mod);
            Database.SaveChanges();

            return Ok();
        }
    }
}
