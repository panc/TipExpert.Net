using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class InvitationStore : IInvitationStore
    {
        private readonly IUserStore _userStore;
        private readonly IGameStore _gameStore;
        private readonly IMongoCollection<Invitation> _collection;

        public InvitationStore(IMongoDatabase database, IUserStore userStore, IGameStore gameStore)
        {
            _userStore = userStore;
            _gameStore = gameStore;
            _collection = database.GetCollection<Invitation>("invitations");
        }

        public async Task Add(Invitation invitation)
        {
            await _collection.InsertOneAsync(invitation);
        }

        public async Task Add(Invitation[] invitations)
        {
            if (invitations != null && invitations.Any())
                await _collection.InsertManyAsync(invitations);
        }

        public async Task Remove(Invitation invitation)
        {
            await _collection.DeleteOneAsync(x => x.Id == invitation.Id);
        }

        public async Task Update(Invitation invitation)
        {
            await _collection.ReplaceOneAsync(x => x.Id == invitation.Id, invitation);
        }

        public async Task<Invitation> GetById(ObjectId id)
        {
            var invitation = await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();

            await _PopulateRelations(invitation);

            return invitation;
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
            if (invitation == null)
                return;

            if (invitation.User == null)
                invitation.User = await _userStore.GetById(invitation.UserId);

            if (invitation.Game == null)
                invitation.Game = await _gameStore.GetById(invitation.GameId);
        }
    }
}