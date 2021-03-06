﻿namespace TipExpert.Net.Models
{
    public class PlayerDto
    {
        public string userId { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public double? stake { get; set; }

        public double? profit { get; set; }

        public int? totalPoints { get; set; }

        public int? ranking { get; set; }
    }
}