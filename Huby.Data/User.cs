using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Huby.Data
{
    public sealed class User : DbItem, ICacheable
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [JsonIgnore]
        public string Email { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        public string Status { get; set; }

        [Required]
        [MinLength(8)]
        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public List<Hub> Hubs { get; set; }

        [JsonIgnore]
        public List<Post> Posts { get; set; }

        [JsonIgnore]
        public List<Subscription> Subscriptions { get; set; }

        [JsonIgnore]
        public List<Moderator> Moderations { get; set; }

        [JsonIgnore]
        public List<Reaction> Reactions { get; set; }

        public DateTime GetLastModified()
        {
            return LastModified;
        }
    }
}
