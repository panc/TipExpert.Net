using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core.Invitation
{
    public interface IInvitationTokeService
    {
        string GetNewInvitaionToken();

        Task UpdatePlayerForToken(string token, ObjectId userId);
    }
}