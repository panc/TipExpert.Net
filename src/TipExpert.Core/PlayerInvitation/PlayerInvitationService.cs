using System.Collections.Generic;
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

        public void SendInvitationsAsync(Game game, InvitedPlayer[] invitedPlayers)
        {
            Task.Run(async () =>
            {
                _UpdateInvitedPlayers(game, invitedPlayers);
                await _SendInvitations(game);
            });
        }

        private void _UpdateInvitedPlayers(Game game, InvitedPlayer[] invitedPlayers)
        {
            if (game.InvitedPlayers == null)
                game.InvitedPlayers = new List<InvitedPlayer>();

            // build a new list to also take removed invitations into account
            game.InvitedPlayers = new List<InvitedPlayer>();

            // TODO:
            // handle delete players -> remove invitation!
            foreach (var invitedPlayer in invitedPlayers)
            {
                var email = invitedPlayer.Email;
                var player = game.InvitedPlayers.FirstOrDefault(x => x.Email == email);

                if (player == null)
                    player = invitedPlayer;

                game.InvitedPlayers.Add(player);
            }
        }

        public async Task UpdateInvitationForPlayer(string token, ObjectId userId)
        {
            var invitationId = token.ToObjectId();
            var invitation = await _invitationStore.GetById(invitationId);
            var game = await _gameStore.GetById(invitation.GameId);

            var invitedPlayer = game?.InvitedPlayers?.SingleOrDefault(x => x.InvitationToken == invitationId);

            if (invitedPlayer != null)
            {
                var player = new Player {UserId = userId};
                game.Players.Add(player);
                game.InvitedPlayers.Remove(invitedPlayer);

                await _gameStore.Update(game);
                await _invitationStore.Remove(invitation);
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