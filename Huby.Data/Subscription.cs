using System;
using System.ComponentModel.DataAnnotations;

namespace Huby.Data
{
    public sealed class Subscription : DbItem, ICacheable
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string HubId { get; set; }

        public User User { get; set; }
        public Hub Hub   { get; set; }

        public DateTime GetLastModified()
        {
            return Helpers.FindMax(LastModified, Hub.GetLastModified(), User.GetLastModified());
        }
    }
}
