using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TipExpert.Core
{
    public class MatchTips
    {
        public Guid MatchId { get; set; }

        [JsonIgnore]
        public Match Match { get; set; }

        public List<Tip> Tips { get; set; }
    }
}