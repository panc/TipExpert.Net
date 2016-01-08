using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class UserStore : IUserStore
    {
        private IMongoCollection<User> _collection;

        public UserStore(IDataStoreConfiguration configuration, MongoClient client)
        {
            var db = client.GetDatabase("TipExpert");
            _collection = db.GetCollection<User>("user");
        }

        public async Task AddUser(User user)
        {
//                if (Entities.Count == 0)
                    user.Role = (int)UserRoles.Admin;

                user.Id = Guid.NewGuid();

            await _collection.InsertOneAsync(user);
        }

        public Task RemoveUser(User user)
        {
            return Task.CompletedTask;
//            return Task.Run(() => Entities.Remove(user));
        }

        public async Task<User[]> GetAll()
        {
            var result = await _collection.FindAsync(FilterDefinition<User>.Empty);
            var list = await result.ToListAsync();
            return list.ToArray();
        }

        public async Task<User> GetById(Guid id)
        {
            var result = await _collection.FindAsync(x => x.Id == id);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<User> FindUserByEmail(string email, CancellationToken cancellationToken)
        {
            email = email.ToUpper();

            var result = await _collection.FindAsync(x => x.Email.ToUpper() == email, cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}