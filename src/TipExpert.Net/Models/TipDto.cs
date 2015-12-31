using System;

namespace TipExpert.Net.Models
{
    public class TipDto
    {
        public Guid userId { get; set; }

        public int homeScore { get; set; }

        public int guestScore { get; set; }

        public int? points { get; set; }
    }
}