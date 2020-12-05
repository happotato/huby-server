using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Huby.Data;
using Huby.Data.Extensions;

namespace Huby.Controllers
{
    [ApiController]
    [LastModifiedCache]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private ILogger<UserController> Logger { get; }
        private ApplicationDatabase Database   { get; }
        private UserManager<User> UserManager  { get; }

        public UserController(ILogger<UserController> logger, ApplicationDatabase db, UserManager<User> um)
        {
            Logger = logger;
            Database = db;
            UserManager = um;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get(
            string name = null,
            string username = null,
            int page = 0,
            int limit = 10,
            string after = null)
        {
            return Database.Users
                .Where(user => name == null || EF.Functions.ILike(user.Name, $"%{name}%"))
                .Where(user => username == null || EF.Functions.ILike(user.Username, $"%{username}%"))
                .OrderBy(user => user.CreatedAt)
                .ToList()
                .Paginate(page, limit, after)
                .ToList();
        }

        [HttpGet("{username}")]
        public ActionResult<User> GetUserByUsername(string username)
        {
            return Database.GetUserByUsername(username);
        }

        [Authorize]
        [HttpPatch("{username}")]
        public ActionResult<User> Patch(string username, JsonElement json)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = Database.GetUserByUsername(username);

            if (user == null)
                return NotFound($"User \"{username}\" not found");

            if (currentUsername != user.Username)
                return Unauthorized();

            if (json.TryGetProperty("name", out JsonElement nameProperty))
            {
                if (nameProperty.ValueKind == JsonValueKind.String)
                {
                    var name = nameProperty.GetString();

                    if (name.Length == 0)
                        return BadRequest("Empty name");

                    user.Name = name;
                }
                else
                {
                    return BadRequest("Property \"name\" must be string");
                }
            }

            if (json.TryGetProperty("email", out JsonElement emailProperty))
            {
                if (emailProperty.ValueKind == JsonValueKind.String)
                {
                    var email = emailProperty.GetString();
                    var emailAttr = new EmailAddressAttribute();

                    if (!emailAttr.IsValid(email))
                        return BadRequest("Invalid email");

                    user.Email = email;
                }
                else
                {
                    return BadRequest("Property \"email\" must be string");
                }
            }

            if (json.TryGetProperty("status", out JsonElement statusProperty))
            {
                if (statusProperty.ValueKind == JsonValueKind.String
                    || statusProperty.ValueKind == JsonValueKind.Null)
                {
                    user.Status = statusProperty.GetString();
                }
                else
                {
                    return BadRequest("Property \"status\" must be string or null");
                }
            }

            if (json.TryGetProperty("imageUrl", out JsonElement imageUrlProperty))
            {
                if (imageUrlProperty.ValueKind == JsonValueKind.String
                    || imageUrlProperty.ValueKind == JsonValueKind.Null)
                {
                    user.ImageUrl = imageUrlProperty.GetString();
                }
                else
                {
                    return BadRequest("Property \"imageUrl\" must be string or null");
                }
            }

            Database.SaveChanges();
            return user;
        }
    }
}
