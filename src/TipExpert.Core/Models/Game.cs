using System;
using System.Collections.Generic;

namespace TipExpert.Core
{
    public class Game
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid CreatorId { get; set; }

        public double MinStake { get; set; }

        public bool IsFinished { get; set; }

        public List<Player> Players { get; set; }

        public List<MatchTips> Matches { get; set; }
    }
}