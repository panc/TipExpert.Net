using System.Collections.Generic;

namespace TipExpert.Net.Models
{
    public class GameDto
    {
        public string id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string creatorId { get; set; }

        public string creator { get; set; }

        public double minStake { get; set; }

        public bool isFinished { get; set; }

        public string  matchSelectionMode { get; set; }

        public string matchesMetadata { get; set; }

        public PlayerDto player { get; set; }
        
        public List<PlayerDto> players { get; set; }

        public List<InvitationDto> invitedPlayers { get; set; }

        public List<MatchTipsDto> matches { get; set; }

        public List<MatchTipsDto> finishedMatches { get; set; }
    }
}