using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TipExpert.Core
{
    public class Invitation
    {
        public ObjectId Id { get; set; }

        public ObjectId GameId { get; set; }

        public ObjectId UserId{ get; set; }

        public string Email { get; set; }

        [BsonIgnore]
        public User User { get; set; }

        [BsonIgnore]
        public Game Game { get; set; }
    }
}