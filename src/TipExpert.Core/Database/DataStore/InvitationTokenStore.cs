using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class InvitationTokenStore : IInvitationTokenStore
    {
        private readonly IMongoCollection<InvitationToken> _collection;

        public InvitationTokenStore(IMongoDatabase database)
        {
            _collection = database.GetCollection<InvitationToken>("invitationTokens");
        }

        public async Task Add(InvitationToken token)
        {
            await _collection.InsertOneAsync(token);
        }

        public async Task Remove(InvitationToken token)
        {
            await _collection.DeleteOneAsync(x => x.Id == token.Id);
        }

        public async Task<InvitationToken> GetById(ObjectId id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}