using System.Net.Mail;

namespace TipExpert.Core.Services
{
    public class MailService
    {
        public void SendInvitation(InvitedPlayer player)
        {
            var email = player.Email;

            MailMessage m = new MailMessage();
            m.Body = "Hello world";
            m.To.Add("christoph.pangerl@gmail.com");

        } 
    }
}