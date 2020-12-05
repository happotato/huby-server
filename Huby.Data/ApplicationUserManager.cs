using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Huby.Data
{
    public sealed class ApplicationUserPrincipalFactory : IUserClaimsPrincipalFactory<User>
    {
        public Task<ClaimsPrincipal> CreateAsync(User user)
        {
            return Task.FromResult(new ClaimsPrincipal());
        }
    }

    public sealed class ApplicationUserStore: IDisposable, IUserStore<User>, IUserEmailStore<User>,
    IUserPasswordStore<User>, IQueryableUserStore<User>
    {
        public readonly ApplicationDatabase Database;

        public ApplicationUserStore(IConfiguration config, ApplicationDatabase database)
        {
            Database = database;
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        IQueryable<User> IQueryableUserStore<User>.Users => Database.Users.AsQueryable();

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            Database.Users.Add(user);
            Database.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            Database.Users.Remove(user);
            Database.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var result = Database.Users
                .Where(user => user.Id == userId)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            return Task.FromResult<User>(result);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = Database.Users
                .Where(user => user.Username == normalizedUserName)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            return Task.FromResult<User>(result);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Username = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            Database.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Password = passwordHash;
            return Task.CompletedTask;
        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var result = Database.Users
                .Where(user => user.Email == normalizedEmail)
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            return Task.FromResult<User>(result);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Email = normalizedEmail;
            return Task.CompletedTask;
        }
    }
}
