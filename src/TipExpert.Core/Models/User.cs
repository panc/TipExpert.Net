using System;

namespace TipExpert.Core
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public int Role { get; set; }

        public double Coins { get; set; }
    }
}