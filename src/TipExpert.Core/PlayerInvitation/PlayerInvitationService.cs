using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core.PlayerInvitation
{
    public class PlayerInvitationService : IPlayerInvitationService
    {
        private readonly IGameStore _gameStore;
        private readonly IInvitationStore _invitationStore;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;

        public PlayerInvitationService(IGameStore gameStore, IInvitationStore invitationStore, string userName, string password, string host, int port)
        {
            _gameStore = gameStore;
            _invitationStore = invitationStore;
            _userName = userName;
            _password = password;
            _host = host;
            _port = port;
        }

        public void SendInvitationsAsync(Game game)
        {
            Task.Run(async () => { await _SendInvitations(game); });
        }
        
        public async Task UpdateInvitationForPlayer(string token, ObjectId userId)
        {
            // for now we search all games for the token...
            // later we will store the game id in the token...
            var games = await _gameStore.GetGamesCreatedByUser(userId);
            var tokenId = token.ToObjectId();

            foreach (var game in games)
            {
                var invitedPlayer = game.InvitedPlayers?.SingleOrDefault(x => x.InvitationToken == tokenId);

                if (invitedPlayer != null)
                {
                    var player = new Player { UserId = userId };
                    game.Players.Add(player);

                    await _gameStore.Update(game);
                    break;
                }
            }
        }

        private async Task _SendInvitations(Game game)
        {
            if (game?.InvitedPlayers == null)
                return;

            foreach (var player in game.InvitedPlayers.Where(x => x.InvitationToken == ObjectId.Empty))
            {
                await _SendInvitation(game, player);
            }

            await _gameStore.Update(game);
        }

        private async Task _SendInvitation(Game game, InvitedPlayer player)
        {
            var token = new Invitation
            {
                GameId = game.Id,
                Email = player.Email
            };

            await _invitationStore.Add(token);

            player.InvitationToken = token.Id;

            MailMessage message = new MailMessage();
            message.Body = string.Format(MESSAGE_BODY, player.InvitationToken);
            message.Subject = MESSAGE_SUBJECT;
            message.To.Add(player.Email);
            message.From = new MailAddress("invitation@tipexpert.net");

            SmtpClient client = new SmtpClient
            {
                Host = _host,
                Port = _port,
                EnableSsl = true,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(_userName, _password)
            };

            await client.SendMailAsync(message);
        }

        private static string MESSAGE_SUBJECT = "Someone invited you to a tip game";
        private static string MESSAGE_BODY = "Hello my friend,\r\n\r\n" +
                                             "Someone invited you to a tip game at TipExpert.Net :-)\r\n\r\n" +
                                             "Check it out at http://tipexpert.azurewebsites.net/token/{0} !\r\n\r\n" +
                                             "Have fun!";
    }
}