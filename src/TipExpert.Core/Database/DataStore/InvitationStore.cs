using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class InvitationStore : IInvitationStore
    {
        private readonly IUserStore _userStore;
        private readonly IMongoCollection<Invitation> _collection;

        public InvitationStore(IMongoDatabase database, IUserStore userStore)
        {
            _userStore = userStore;
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

        public async Task Update(Invitation invitation)
        {
            await _collection.ReplaceOneAsync(x => x.Id == invitation.Id, invitation);
        }

        public async Task<Invitation> GetById(ObjectId id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Invitation[]> GetInvitationsForGame(ObjectId gameId)
        {
            var invitations = await _collection
                .Find(x => x.GameId == gameId)
                .ToArrayAsync();

            await _PopulateRelations(invitations);

            return invitations;
        }

        private async Task _PopulateRelations(Invitation[] invitations)
        {
            if (invitations == null)
                return;

            foreach (var invitation in invitations)
                await _PopulateRelations(invitation);
        }

        private async Task _PopulateRelations(Invitation invitation)
        {
            if (invitation?.User != null)
                invitation.User = await _userStore.GetById(invitation.UserId);
        }
    }
}