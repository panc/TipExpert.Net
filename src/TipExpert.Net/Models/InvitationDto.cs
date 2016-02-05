namespace TipExpert.Net.Models
{
    public class InvitationDto
    {
        public string id { get; set; }

        public string gameId { get; set; }

        public string email { get; set; }

        public string name { get; set; }

        public string error { get; set; }

        public int state { get; set; }
    }
}