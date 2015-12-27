using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using TipExpert.Core;

namespace TipExpert.Net.Authentication
{
    public class ApplicationUserStore : IUserStore<ApplicationUser>
    {
        private readonly UserStore _userStore;

        public ApplicationUserStore(UserStore userStore)
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

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName,
            CancellationToken cancellationToken)
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

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindUserByEmail(normalizedUserName, cancellationToken);

            if (user == null)
                return null;

            return new ApplicationUser
            {
                Email = user.Email,
                UserName = user.UserName
            };
        }
    }
}