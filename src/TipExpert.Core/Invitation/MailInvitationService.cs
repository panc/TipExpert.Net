using System.Net.Mail;

namespace TipExpert.Core.Invitation
{
    public class MailInvitationService : IMailInvitationService
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;

        public MailInvitationService(string userName, string password, string host, int port)
        {
            _userName = userName;
            _password = password;
            _host = host;
            _port = port;
        }

        public void SendInvitation(InvitedPlayer player)
        {
            var email = player.Email;

            MailMessage message = new MailMessage();
            message.Body = "Hello world";
            message.Subject = "First test";
            message.To.Add(email);
            message.From = new MailAddress("invitation@tipexpert.net");

            SmtpClient client = new SmtpClient();
            client.Host = _host;
            client.Port = _port;
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(_userName, _password);

            client.Send(message);
        } 
    }
}