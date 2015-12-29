using System;

namespace TipExpert.Net.Models
{
    public class MatchDto
    {
        public Guid id { get; set; }
        
        public Guid leagueId { get; set; }

        public string homeTeam { get; set; }

        public string guestTeam { get; set; }

        public int homeScore { get; set; }

        public int guestScore { get; set; }

        public string dueDate { get; set; }
    }
}