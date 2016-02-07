using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core
{
    public interface IInvitationStore
    {
        Task Add(Invitation invitation);

        Task Add(Invitation[] invitations);

        Task Remove(Invitation invitation);

        Task Update(Invitation invitation);

        Task<Invitation> GetById(ObjectId id);

        Task<Invitation[]> GetInvitationsForGame(ObjectId gameId);
    }
}