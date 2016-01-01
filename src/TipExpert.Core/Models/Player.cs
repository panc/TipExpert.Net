using System;
using Newtonsoft.Json;

namespace TipExpert.Core
{
    public class Player
    {
        public Guid UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public double? Stake { get; set; }

        public double? Profit { get; set; }

        public int? TotalPoints { get; set; }
    }
}