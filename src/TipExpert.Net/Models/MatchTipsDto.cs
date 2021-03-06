﻿using System.Collections.Generic;

namespace TipExpert.Net.Models
{
    public class MatchTipsDto
    {
        public string matchId { get; set; }

        public MatchDto match { get; set; }

        public TipDto tipOfPlayer { get; set; }

        public List<TipDto> tips { get; set; }
    }
}