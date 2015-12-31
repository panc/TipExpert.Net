using System;
using System.Collections.Generic;

namespace TipExpert.Net.Models
{
    public class MatchTipsDto
    {
        public Guid matchId { get; set; }

        public List<TipDto> tips { get; set; }
    }
}