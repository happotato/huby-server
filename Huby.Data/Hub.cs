using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Huby.Data.Extensions;

namespace Huby.Data
{
    public sealed class Hub : DbItem, ICacheable
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Url]
        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public bool IsNSFW { get; set; }

        [Required]
        public int BannerColor { get; set; }

        [Required]
        public long SubscribersCount { get; set; }

        public User Owner { get; set; }

        [JsonIgnore]
        public List<Post> Topics { get; set; }

        [JsonIgnore]
        public List<Subscription> Subscriptions { get; set; }

        [JsonIgnore]
        public List<Moderator> Moderators { get; set; }

        public DateTime GetLastModified()
        {
            return Helpers.FindMax(LastModified, Owner.LastModified);
        }

        public Permissions GetPermissions(string userId)
        {
            if (OwnerId == userId)
                return Permissions.Owner;

            return Moderators
                .Where(mod => mod.UserId == userId)
                .ToList()
                .Select(mod => mod.ToPermissions())
                .DefaultIfEmpty(new Permissions())
                .SingleOrDefault();
        }

        public bool CanEdit(string userId)
        {
            var p = GetPermissions(userId);
            return p.CanEdit;
        }

        public bool CanDeletePosts(string userId)
        {
            var p = GetPermissions(userId);
            return p.CanDeletePosts;
        }

        public bool IsSubscribed(string userId)
        {
            return Subscriptions
                .Any(sub => sub.UserId == userId);
        }
    }
}
