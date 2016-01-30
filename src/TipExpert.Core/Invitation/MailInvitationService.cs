using System.Net.Mail;

namespace TipExpert.Core.Invitation
{
    public class MailInvitationService : IMailInvitationService
    {
        public void SendInvitation(InvitedPlayer player)
        {
            var email = player.Email;

            MailMessage message = new MailMessage();
            message.Body = "Hello world";
            message.Subject = "First test";
            message.To.Add(email);
            message.From = new MailAddress("invitation@tipexpert.net");

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("dummy", "dummy");

            client.Send(message);
        } 
    }
}