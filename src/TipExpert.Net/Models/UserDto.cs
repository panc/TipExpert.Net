using System;

namespace TipExpert.Net.Models
{
    public class UserDto
    {
        public Guid id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public int role { get; set; }

        public int coins { get; set; }
    }
}