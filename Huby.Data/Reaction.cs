using System;
using System.ComponentModel.DataAnnotations;

namespace Huby.Data
{
    public enum ReactionValue
    {
        None,
        Like,
        Dislike,
    };

    public sealed class Reaction : DbItem, ICacheable
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string PostId { get; set; }

        [Required]
        public ReactionValue Value { get; set; }

        public User User { get; set; }
        public Post Post { get; set; }

        public DateTime GetLastModified()
        {
            return Helpers.FindMax(LastModified, Post.GetLastModified(), User.GetLastModified());
        }
    }
}
