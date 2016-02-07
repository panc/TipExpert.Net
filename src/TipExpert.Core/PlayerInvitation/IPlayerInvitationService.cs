using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core.PlayerInvitation
{
    public interface IPlayerInvitationService
    {
        Task SendInvitationsAsync(Game game, Invitation[] map);

        Task AcceptInvitation(string token, ObjectId userId);

        Task<Invitation[]> GetInvitatationsForGame(ObjectId gameId);
    }
}