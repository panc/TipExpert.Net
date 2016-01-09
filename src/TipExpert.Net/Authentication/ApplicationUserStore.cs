using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using TipExpert.Core;

namespace TipExpert.Net.Authentication
{
    public class ApplicationUserStore : IUserPasswordStore<ApplicationUser>
    {
        private readonly IUserStore _userStore;

        public ApplicationUserStore(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(appUser.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(appUser.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser appUser, string userName, CancellationToken cancellationToken)
        {
            appUser.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(appUser.Email);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser appUser, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            var existingUser = await _userStore.FindUserByEmail(appUser.Email, cancellationToken);

            if (existingUser != null)
                return IdentityResult.Failed(new IdentityError() {Code = "1", Description = "User with the same Email-Adress already exists!"});

            var user = new User
            {
                Name = appUser.UserName,
                Email = appUser.Email,
                PasswordHash = appUser.PasswordHash,
                Role = (int)UserRoles.User
            };

            await _userStore.AddUser(user);

            appUser.Id = user.Id;

            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userStore.GetById(Guid.Parse(userId));

            if (user == null)
                return null;

            return new ApplicationUser
            {
                Email = user.Email,
                UserName = user.Name
            };
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(normalizedUserName, cancellationToken);

            if (user == null)
                return null;

            return new ApplicationUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.Name,
                PasswordHash = user.PasswordHash
            };
        }

        public Task SetPasswordHashAsync(ApplicationUser appUser, string passwordHash, CancellationToken cancellationToken)
        {
            appUser.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(appUser.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(appUser.PasswordHash));
        }
    }
}