using System;
using System.Collections.Generic;

namespace TipExpert.Net.Models
{
    public class GameDto
    {
        public Guid id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public Guid creatorId { get; set; }

        public double minStake { get; set; }

        public bool isFinished { get; set; }

        public List<PlayerDto> players { get; set; }

        public List<MatchTipsDto> matches { get; set; }
    }
}