using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core.Invitation
{
    public interface IInvitationService
    {
        void SendInvitationsAsync(Game game);

        Task UpdateInvitationForPlayer(string token, ObjectId userId);
    }
}