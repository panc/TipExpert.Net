using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class UserStore : IUserStore
    {
        private readonly IMongoCollection<User> _collection;

        public UserStore(MongoClient client)
        {
            var db = client.GetDatabase("TipExpert");
            _collection = db.GetCollection<User>("user");
        }

        public async Task AddUser(User user)
        {
            if (_collection.Count(FilterDefinition<User>.Empty) == 0)
                user.Role = (int)UserRoles.Admin;

            user.Id = Guid.NewGuid();

            await _collection.InsertOneAsync(user);
        }

        public async Task RemoveUser(User user)
        {
            await _collection.DeleteOneAsync(x => x.Id == user.Id);
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
    }
}