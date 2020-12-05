using System;
using System.Text;    
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;    
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;   
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Huby.Data;

namespace Huby.Controllers
{
    public class LogInData
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }

    public sealed class SignUpData : LogInData
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public sealed class UserToken
    {
        public User User    { get; }
        public string Token { get; }

        public UserToken(User user, string key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));    
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    

            var handler = new JwtSecurityTokenHandler();

            var claims = new[] {    
                new Claim(ClaimTypes.Name, user.Username),    
                new Claim(ClaimTypes.Sid, user.Id),    
            }; 
    
            var token = new JwtSecurityToken("Issuer",    
                "Audience",
                claims,    
                expires: DateTime.Now.AddDays(30),    
                signingCredentials: credentials);    
    
            Token = handler.WriteToken(token);
            User = user;
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private ILogger<AuthController> Logger        { get; }
        private ApplicationDatabase     Database      { get; }
        private UserManager<User>       UserManager   { get; }
        private IConfiguration          Configuration { get; }
        private String                  JWTKey        { get; }

        public AuthController(ILogger<AuthController> logger, ApplicationDatabase db, UserManager<User> um, IConfiguration conf)
        {
            Logger = logger;
            Database = db;
            UserManager = um;
            Configuration = conf;
            JWTKey = conf["JWT:Key"];
        }

        [Authorize]
        [HttpPost("jwt")]
        public ActionResult<UserToken> Auth()
        {
            var username = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            var user = Database.Users
                .Where(u => u.Username == username)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (user == null)
                return NotFound("User not found");

            return Ok(new UserToken(user, JWTKey));
        }

        [HttpPost("jwt/create")]
        public ActionResult<UserToken> SignUp(SignUpData data)
        {
            var user = new User
            {
                Id = DbItem.GenerateID(),
                Name = data.Name,
                Username = data.Username,
                Email = data.Email,
                Password = data.Password
            };

            if (Database.Users.Any(u => u.Username == user.Username))
                return Conflict("Username already exists");

            Database.Users.Add(user);
            Database.SaveChanges();

            return SignIn(data);
        }

        [HttpPost("jwt/signin")]
        public ActionResult<UserToken> SignIn(LogInData data)
        {
            var user = Database.Users
                .Where(user => user.Username == data.Username)
                .Where(user => user.Password == data.Password)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (user == null)
                return NotFound("User not found");

            return Ok(new UserToken(user, JWTKey));
        }
    }
}
