using System;
using System.ComponentModel.DataAnnotations;

namespace Huby.Data
{
    public interface IIdentifiable<T>
    {
        public T Id { get; }
    }

    public abstract class DbItem : IIdentifiable<string>
    {
        [Key]
        [Required]
        [StringLength(12)]
        public string Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public static string GenerateID()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace('/', '-')
                .Replace('+', '_')
                .Substring(0, 12);
        }
    }
}
