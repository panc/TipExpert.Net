namespace TipExpert.Net.Models
{
    public class MatchDto
    {
        public string id { get; set; }
        
        public string leagueId { get; set; }

        public string homeTeam { get; set; }

        public string guestTeam { get; set; }

        public int homeScore { get; set; }

        public int guestScore { get; set; }

        public string dueDate { get; set; }

        public bool isFinished { get; set; }
    }
}