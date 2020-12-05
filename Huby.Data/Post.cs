using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Huby.Data
{
    public enum ContentType
    {
        Markdown,
        Image,
    }

    public abstract class Post : DbItem, ICacheable
    {
        [Required]
        public long Likes { get; set; }

        [Required]
        public long Dislikes { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public ContentType ContentType { get; set; }

        [Required]
        public bool IsNSFW { get; set; }

        [Required]
        public long CommentsCount { get; set; }

        [Required]
        public string PostType { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string HubId { get; set; }

        public User Owner { get; set; }

        public Hub Hub { get; set; }

        [JsonIgnore]
        public List<Comment> Comments { get; set; }

        [JsonIgnore]
        public List<Reaction> Reactions { get; set; }

        public abstract DateTime GetLastModified();

        public bool CanDelete(string userId)
        {
            return OwnerId == userId ||
                Hub.CanDeletePosts(userId);
        }

        public ReactionValue GetReactionValue(string userId)
        {
            if (userId == null)
                return ReactionValue.None;

            var reaction = Reactions
                .Where(reaction => reaction.UserId == userId)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();

            if (reaction == null)
                return ReactionValue.None;

            return reaction.Value;
        }
    }

    public sealed class Topic : Post
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Stickied { get; set; }

        [Required]
        public bool Locked { get; set; }

        [Required]
        public string Tags { get; set; }

        public override DateTime GetLastModified()
        {
            return Helpers.FindMax(LastModified, Hub.GetLastModified(), Owner.GetLastModified());
        }
    }

    public sealed class Comment : Post, ICacheable
    {
        [Required]
        public string ParentId { get; set; }

        public Post Parent { get; set; }

        public override DateTime GetLastModified()
        {
            Post current = this;
            var dt = LastModified;

            while (current is Comment comment && comment.Parent is Post parent)
            {
                if (comment.LastModified < parent.LastModified)
                    dt = parent.LastModified;

                current = parent;
            }

            return Helpers.FindMax(dt, Hub.GetLastModified(), Owner.GetLastModified());
        }
    }
}
