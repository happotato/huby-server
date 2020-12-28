using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Huby.Data
{
    public sealed class ApplicationDatabase : DbContext
    {
        public DbSet<User> Users                 { get; private set; }
        public DbSet<Hub> Hubs                   { get; private set; }
        public DbSet<Post> Posts                 { get; private set; }
        public DbSet<Topic> Topics               { get; private set; }
        public DbSet<Comment> Comments           { get; private set; }
        public DbSet<Subscription> Subscriptions { get; private set; }
        public DbSet<Moderator> Moderators       { get; private set; }
        public DbSet<Reaction> Reactions         { get; private set; }
        public ISimpleCrypto Crypto              { get; private set; }

        public ApplicationDatabase(DbContextOptions options, ISimpleCrypto cConfig) : base(options)
        {
            Crypto = cConfig;
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<DbItem>()
                .Where(item => item.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.LastModified = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(user => user.Password)
                .HasConversion<string>(
                    (pwd) => Encoding.Unicode.GetString(Crypto.Encrypt(pwd)),
                    (pwd) => Crypto.Decrypt(Encoding.Unicode.GetBytes(pwd))
                );

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder.Entity<Hub>()
                .HasIndex(hub => hub.Name)
                .IsUnique();

            modelBuilder.Entity<Hub>()
                .HasOne(hub => hub.Owner)
                .WithMany(user => user.Hubs)
                .HasForeignKey(hub => hub.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Moderator>()
                .HasOne(mod => mod.User)
                .WithMany(user => user.Moderations)
                .HasForeignKey(mod => mod.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Moderator>()
                .HasOne(mod => mod.Hub)
                .WithMany(hub => hub.Moderators)
                .HasForeignKey(mod => mod.HubId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Subscription>()
                .HasOne(sub => sub.User)
                .WithMany(user => user.Subscriptions)
                .HasForeignKey(sub => sub.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Subscription>()
                .HasOne(sub => sub.Hub)
                .WithMany(hub => hub.Subscriptions)
                .HasForeignKey(sub => sub.HubId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Reaction>()
                .HasOne(reaction => reaction.User)
                .WithMany(user => user.Reactions)
                .HasForeignKey(reaction => reaction.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Reaction>()
                .HasOne(reaction => reaction.Post)
                .WithMany(post => post.Reactions)
                .HasForeignKey(reaction => reaction.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .HasIndex(post => post.PostType);

            modelBuilder.Entity<Post>()
                .HasDiscriminator(post => post.PostType)
                .HasValue<Topic>(nameof(Topic))
                .HasValue<Comment>(nameof(Comment));

            modelBuilder.Entity<Post>()
                .HasOne(post => post.Owner)
                .WithMany(user => user.Posts)
                .HasForeignKey(post => post.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .HasOne(topic => topic.Hub)
                .WithMany(hub => hub.Topics)
                .HasForeignKey(topic => topic.HubId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Comment>()
                .HasOne(cmt => cmt.Parent)
                .WithMany(post => post.Comments)
                .HasForeignKey(cmt => cmt.ParentId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }

        public User GetUserByUsername(string username)
        {
            if (username == null)
                return null;

            return Users
                .Where(u => u.Username == username)
                .ToList()
                .DefaultIfEmpty(null)
                .SingleOrDefault();
        }
    }
}
