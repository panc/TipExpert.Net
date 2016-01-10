using MongoDB.Bson;

namespace TipExpert.Core
{
    public class Match
    {
        public ObjectId Id { get; set; }

        public ObjectId LeagueId { get; set; }

        public string HomeTeam { get; set; }

        public string GuestTeam { get; set; }

        public int? HomeScore { get; set; }

        public int? GuestScore { get; set; }

        public string DueDate { get; set; }

        public bool IsFinished { get; set; }
    }
}