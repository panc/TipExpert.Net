using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core.PlayerInvitation
{
    public interface IPlayerInvitationService
    {
        void SendInvitationsAsync(Game game);

        Task UpdateInvitationForPlayer(string token, ObjectId userId);
    }
}