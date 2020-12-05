using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Huby.Data;
using Huby.Data.Extensions;

namespace Huby.Controllers
{
    public sealed class CommentCreateData
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public ContentType ContentType { get; set; }

        [Required]
        public bool IsNSFW { get; set; }
    }

    public sealed class PostQueryResult
    {
        public ReactionValue Reaction { get; set; }
        public object Post { get; set; }
    }

    [ApiController]
    [LastModifiedCache]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private ILogger<PostController> Logger { get; }
        private ApplicationDatabase Database   { get; }

        public PostController(ILogger<PostController> logger, ApplicationDatabase db)
        {
            Logger = logger;
            Database = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> Get(
            string title = null,
            string hub = null,
            string parent = null,
            string owner = null,
            string type = null,
            string sort = "new",
            string after = null,
            int limit = 10,
            int page = 0)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.Users
                .Include(user => user.Subscriptions)
                .Where(user => user.Username == currentUsername)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            IEnumerable<Post> posts = Database.Posts
                .Include(post => post.Owner)
                .Include(post => post.Comments)
                .Include(post => post.Reactions)
                .Include(post => post.Hub.Owner)
                .Include(post => (post as Comment).Parent.Owner)
                .Where(post => type == null || post.PostType == type)
                .Where(post => owner == null || post.Owner.Username == owner);

            switch (hub) {
                case "_subscriptions": {
                    if (currentUser == null)
                        break;

                    posts = posts
                        .Where(post => currentUser.Subscriptions.Any(sub => post.HubId == sub.HubId));
                } break;

                default: {
                    posts = posts
                        .Where(post => hub == null || post.Hub.Name == hub);
                } break;
            }

            switch (sort)
            {
                case "top": {
                    posts = posts
                        .OrderByDescending(post => post.Likes - post.Dislikes);
                } break;

                case "hot": {
                    var since = DateTime.Now.AddDays(-7);
                    posts = posts
                        .OrderByDescending(post => post.Reactions.Where(r => r.CreatedAt > since).Count());
                } break;

                default: {
                    posts = posts
                        .OrderByDescending(post => post.CreatedAt);
                } break;
            }

            if (title != null)
            {
                posts = posts
                    .OfType<Topic>()
                    .Where(topic => EF.Functions.ILike(topic.Title, $"%{title}%"));
            }
            else if (parent != null)
            {
                posts = posts
                    .OfType<Comment>()
                    .Where(cmt => cmt.ParentId == parent);
            }

            return posts 
                .Paginate(page, limit, after)
                .Select(post => new PostQueryResult
                {
                    Reaction = post.GetReactionValue(currentUser?.Id),
                    Post = post,
                })
                .ToList<object>();
        }

        [HttpGet("{id}")]
        public ActionResult<object> GetPostById(string id)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.GetUserByUsername(currentUsername);

            var post = Database.Posts
                .Include(post => post.Owner)
                .Include(post => post.Reactions)
                .Include(post => post.Hub.Owner)
                .Include(post => (post as Comment).Parent.Owner)
                .Where(post => post.Id == id)
                .ToList()
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (post == null)
                return NotFound($"Post \"{id}\" not found");

            return new PostQueryResult
            {
                Reaction = post.GetReactionValue(currentUser?.Id),
                Post = post,
            };
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult<object> Delete(string id)
        {
            var currentUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var currentUser = Database.GetUserByUsername(currentUsername);

            var post = Database.Posts
                .Include(post => post.Hub.Moderators)
                .Include(post => (post as Comment).Parent)
                .Where(post => post.Id == id)
                .ToList()
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (post == null)
                return NotFound($"Post \"{id}\" not found");

            if (!post.CanDelete(currentUser.Id))
                return Unauthorized();

            if (post is Comment cmt)
                cmt.Parent.CommentsCount -= 1;

            Database.Posts.Remove(post);
            Database.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/comments")]
        public ActionResult<Comment> CreateComment(string id, CommentCreateData data)
        {
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = Database.GetUserByUsername(username);

            var post = Database.Posts
                .Include(post => post.Owner)
                .Include(post => post.Hub.Owner)
                .Where(post => post.Id == id)
                .ToList()
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (post == null)
                return NotFound($"Post \"{id}\" not found");

            post.CommentsCount += 1;

            var comment = new Comment
            {
                Id = DbItem.GenerateID(),
                Content = data.Content,
                ContentType = data.ContentType,
                IsNSFW = data.IsNSFW,
                Parent = post,
                ParentId = post.Id,
                HubId = post.Hub.Id,
                Hub = post.Hub,
                OwnerId = user.Id,
                Owner = user,
            };

            Database.Posts.Add(comment);
            Database.SaveChanges();

            return Created($"/api/post/{comment.Id}", comment);
        }

        [Authorize]
        [HttpPost("{id}/react/like")]
        public ActionResult<object> LikeReaction(string id)
        {
            return React(id, ReactionValue.Like);
        }

        [Authorize]
        [HttpPost("{id}/react/dislike")]
        public ActionResult<object> DislikeReaction(string id)
        {
            return React(id, ReactionValue.Dislike);
        }

        [Authorize]
        [HttpPost("{id}/react/clear")]
        public ActionResult<object> ClearReaction(string id)
        {
            return React(id, ReactionValue.None);
        }

        private ActionResult<object> React(string id, ReactionValue value)
        {
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = Database.GetUserByUsername(username);

            var post = Database.Posts
                .Include(post => post.Owner)
                .Include(post => post.Hub.Owner)
                .Include(post => post.Reactions)
                .Include(post => (post as Comment).Parent)
                .Where(post => post.Id == id)
                .ToList()
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (post == null)
                return NotFound($"Post \"{id}\" not found");

            var oldReaction = post.Reactions
                .Where(reaction => reaction.UserId == user.Id)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (oldReaction != null)
            {
                post.Reactions.Remove(oldReaction);
            }

            if (value != ReactionValue.None)
            {
                post.Reactions.Add(new Reaction
                {
                    Id = DbItem.GenerateID(),
                    Post = post,
                    PostId = post.Id,
                    User = user,
                    UserId = user.Id,
                    Value = value,
                });
            }

            post.Likes = post.Reactions
                .Where(reaction => reaction.Value == ReactionValue.Like)
                .Count();

            post.Dislikes = post.Reactions
                .Where(reaction => reaction.Value == ReactionValue.Dislike)
                .Count();

            Database.SaveChanges();

            return Ok(post);
        }
    }
}
