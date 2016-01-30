using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core.Invitation
{
    public class InvitationTokeService : IInvitationTokeService
    {
        private readonly IGameStore _gameStore;

        public InvitationTokeService(IGameStore gameStore)
        {
            _gameStore = gameStore;
        }

        public string GetNewInvitaionToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task UpdatePlayerForToken(string token, ObjectId userId)
        {
            // for now we search all games for the token...
            // later we will store the game id in the token...
            var games = await _gameStore.GetGamesCreatedByUser(userId);

            foreach (var game in games)
            {
                var invitedPlayer = game.InvitedPlayers?.SingleOrDefault(x => x.InvitationToken == token);

                if (invitedPlayer != null)
                {
                    var player = new Player { UserId = userId };
                    game.Players.Add(player);

                    await _gameStore.Update(game);
                    break;
                }
            }
        }
    }
}