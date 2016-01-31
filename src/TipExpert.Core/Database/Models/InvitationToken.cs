using MongoDB.Bson;

namespace TipExpert.Core
{
    public class InvitationToken
    {
        public ObjectId Id { get; set; }

        public ObjectId GameId { get; set; }

        public string Email { get; set; }
    }
}