using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TipExpert.Core
{
    public class MatchTips
    {
        public ObjectId MatchId { get; set; }

        [BsonIgnore]
        public Match Match { get; set; }

        public List<Tip> Tips { get; set; }
    }
}