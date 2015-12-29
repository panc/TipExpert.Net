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

        public async Task<string> GetUserIdAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(appUser.Email, cancellationToken);
            return user.Id.ToString();
        }

        public async Task<string> GetUserNameAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(appUser.Email, cancellationToken);
            return user.Name;
        }

        public async Task SetUserNameAsync(ApplicationUser appUser, string userName, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(appUser.Email, cancellationToken);
            user.Name = userName;

            await _userStore.SaveChangesAsync();
        }

        public async Task<string> GetNormalizedUserNameAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(appUser.Email, cancellationToken);
            return user.Email;
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
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
                Email = user.Email,
                UserName = user.Name
            };
        }

        public async Task SetPasswordHashAsync(ApplicationUser appUser, string passwordHash, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(appUser.Email, cancellationToken);
            user.PasswordHash = passwordHash;

            await _userStore.SaveChangesAsync();
        }

        public async Task<string> GetPasswordHashAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(appUser.Email, cancellationToken);
            return user.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(ApplicationUser appUser, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(appUser.Email, cancellationToken);
            return !string.IsNullOrEmpty(user.PasswordHash);
        }
    }
}