using System.Net.Mail;

namespace TipExpert.Core.Invitation
{
    public class MailInvitationService : IMailInvitationService
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly IInvitationTokeService _tokenService;

        public MailInvitationService(IInvitationTokeService tokenService, string userName, string password, string host, int port)
        {
            _userName = userName;
            _password = password;
            _host = host;
            _port = port;
            _tokenService = tokenService;
        }

        public void SendInvitation(InvitedPlayer player)
        {
            player.InvitationToken = _tokenService.GetNewInvitaionToken();

            MailMessage message = new MailMessage();
            message.Body = string.Format(MESSAGE_BODY, player.InvitationToken);
            message.Subject = MESSAGE_SUBJECT;
            message.To.Add(player.Email);
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

        private static string MESSAGE_SUBJECT = "Someone invited you to a tip game";
        private static string MESSAGE_BODY = "Hello my friend,\r\n\r\n" +
                                             "Someone invited you to a tip game at TipExpert.Net :-)\r\n\r\n" +
                                             "Check it out at http://tipexpert.azurewebsites.net/token/{0} !\r\n\r\n" +
                                             "Have fun!";
    }
}