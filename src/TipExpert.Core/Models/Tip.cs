using System;

namespace TipExpert.Core
{
    public class Tip
    {
        public Guid UserId { get; set; }

        public int HomeScore { get; set; }

        public int GuestScore { get; set; }

        public int? Points { get; set; }
    }
}