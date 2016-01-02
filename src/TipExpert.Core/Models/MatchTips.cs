using System;
using System.Collections.Generic;

namespace TipExpert.Core
{
    public class MatchTips
    {
        public Guid MatchId { get; set; }

        public Match Match { get; set; }

        public List<Tip> Tips { get; set; }
    }
}