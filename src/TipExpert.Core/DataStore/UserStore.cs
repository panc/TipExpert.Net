using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public class UserStore : StoreBase<User>, IUserStore
    {
        private const string FILE_NAME = "user.json";

        public UserStore(string appDataPath)
            : base(appDataPath, FILE_NAME)
        {
        }

        public Task AddUser(User user)
        {
            return Task.Run(() =>
            {
                user.Id = Guid.NewGuid();
                Entities.Add(user);
            });
        }

        public Task RemoveUser(User user)
        {
            return Task.Run(() => Entities.Remove(user));
        }

        public Task<User[]> GetAll()
        {
            return Task.Run(() => Entities.ToArray());
        }

        public Task<User> GetById(Guid id)
        {
            return Task.Run(() =>
            {
                return Entities.FirstOrDefault(x => x.Id == id);
            });
        }

        public Task<User> FindUserByEmail(string email, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                return Entities.FirstOrDefault(x => x.Email == email);

            }, cancellationToken);
        }
    }
}