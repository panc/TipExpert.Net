using System.Collections.Generic;
using MongoDB.Bson;

namespace TipExpert.Core
{
    public class Game
    {
        public ObjectId Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ObjectId CreatorId { get; set; }

        public double MinStake { get; set; }

        public bool IsFinished { get; set; }

        public MatchSelectionMode MatchSelectionMode { get; set; }

        public string MatchesMetadata { get; set; }
        
        public List<Player> Players { get; set; }

        public List<MatchTips> Matches { get; set; }
    }
}
