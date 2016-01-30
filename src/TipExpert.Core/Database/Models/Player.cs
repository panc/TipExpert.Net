using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TipExpert.Core
{
    public class Player
    {
        public ObjectId UserId { get; set; }

        [BsonIgnore]
        public User User { get; set; }

        public bool InvitationAccepted { get; set; }

        public double? Stake { get; set; }

        public double? Profit { get; set; }

        public int? TotalPoints { get; set; }

        public int? Ranking { get; set; }
    }
}