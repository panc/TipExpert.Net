using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class InvitationStore : IInvitationStore
    {
        private readonly IMongoCollection<Invitation> _collection;

        public InvitationStore(IMongoDatabase database)
        {
            _collection = database.GetCollection<Invitation>("invitations");
        }

        public async Task Add(Invitation token)
        {
            await _collection.InsertOneAsync(token);
        }

        public async Task Remove(Invitation token)
        {
            await _collection.DeleteOneAsync(x => x.Id == token.Id);
        }

        public async Task<Invitation> GetById(ObjectId id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Invitation[]> GetInvitationsForGame(ObjectId gameId)
        {
            return await _collection
                .Find(x => x.GameId == gameId)
                .ToArrayAsync();
        }
    }
}