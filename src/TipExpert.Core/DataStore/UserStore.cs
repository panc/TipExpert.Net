using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class UserStore : IUserStore
    {
        private readonly IMongoCollection<User> _collection;

        public UserStore(IMongoDatabase database)
        {
            _collection = database.GetCollection<User>("user");
        }

        public async Task AddUser(User user)
        {
            if (_collection.Count(FilterDefinition<User>.Empty) == 0)
                user.Role = (int)UserRoles.Admin;

            await _collection.InsertOneAsync(user);
        }

        public async Task RemoveUser(User user)
        {
            await _collection.DeleteOneAsync(x => x.Id == user.Id);
        }

        public async Task Update(User user)
        {
            await _collection.ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        public async Task<User[]> GetAll()
        {
            return await _collection
                .Find(FilterDefinition<User>.Empty)
                .ToArrayAsync();
        }

        public async Task<User> GetById(ObjectId id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<User> FindUserByEmail(string email, CancellationToken cancellationToken)
        {
            return await _collection
                .Find(x => x.Email.ToUpper() == email.ToUpper())
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}