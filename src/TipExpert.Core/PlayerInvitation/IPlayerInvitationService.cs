using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core.PlayerInvitation
{
    public interface IPlayerInvitationService
    {
        Task SendInvitationsAsync(Game game, Invitation[] map);

        Task AcceptInvitation(Invitation invitation, ObjectId userId);

        Task<Invitation> GetInvitatationsForToken(string token);

        Task<Invitation[]> GetInvitatationsForGame(ObjectId gameId);
    }
}