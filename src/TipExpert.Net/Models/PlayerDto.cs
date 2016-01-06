using System;

namespace TipExpert.Net.Models
{
    public class PlayerDto
    {
        public Guid userId { get; set; }

        public string name { get; set; }

        public double? stake { get; set; }

        public double? profit { get; set; }

        public int? totalPoints { get; set; }

        public int? ranking { get; set; }
    }
}