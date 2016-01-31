using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TipExpert.Core
{
    public class InvitedPlayer
    {
        public ObjectId UserId { get; set; }

        [BsonIgnore]
        public User User { get; set; }

        public string Email { get; set; }

        public ObjectId InvitationToken { get; set; }
    }
}