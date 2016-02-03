using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core
{
    public interface IInvitationStore
    {
        Task Add(Invitation token);

        Task Remove(Invitation token);

        Task<Invitation> GetById(ObjectId id);

        Task<Invitation[]> GetInvitationsForGame(ObjectId gameId);
    }
}