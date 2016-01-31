using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core
{
    public interface IInvitationTokenStore
    {
        Task Add(InvitationToken token);

        Task Remove(InvitationToken token);

        Task<InvitationToken> GetById(ObjectId id);
    }
}