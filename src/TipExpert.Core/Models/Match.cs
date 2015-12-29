using System;

namespace TipExpert.Core
{
    public class Match
    {
        public Guid Id { get; set; }

        public Guid LeagueId { get; set; }

        public string HomeTeam { get; set; }

        public string GuestTeam { get; set; }

        public int HomeScore { get; set; }

        public int GuestScore { get; set; }

        public string DueDate { get; set; }
    }
}