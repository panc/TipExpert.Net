using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace TipExpert.Core.PlayerInvitation
{
    public class PlayerInvitationService : IPlayerInvitationService
    {
        private readonly IGameStore _gameStore;
        private readonly IInvitationStore _invitationStore;
        private readonly ILogger<PlayerInvitationService> _logger;

        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _smtpHost;
        private readonly int _smtpPort;

        public PlayerInvitationService(IGameStore gameStore, IInvitationStore invitationStore, ILogger<PlayerInvitationService> logger,
            string hostName, string userName, string password, string smtpHost, int smtpPort)
        {
            _gameStore = gameStore;
            _invitationStore = invitationStore;
            _logger = logger;
            _hostName = hostName;
            _userName = userName;
            _password = password;
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
        }

        public async Task SendInvitationsAsync(Game game, Invitation[] invitedPlayers)
        {
            var newInvitations = await _UpdateInvitedPlayers(game, invitedPlayers);

            // send invitations asynchronously as sending a mail might take a while
            _SendInvitationsAsync(game, newInvitations);
        }

        public async Task AcceptInvitation(Invitation invitation, ObjectId userId)
        {
            _logger.LogInformation($"User '{userId}' has accepted invitation '{invitation.Id}'.");

            // add player to game
            var player = new Player { UserId = userId };
            invitation.Game.Players.Add(player);

            await _gameStore.Update(invitation.Game);

            // remove invitation
            await _invitationStore.Remove(invitation);
        }

        public async Task<Invitation> GetInvitatationsForToken(string token)
        {
            var invitationId = token.ToObjectId();
            return await _invitationStore.GetById(invitationId);
        }

        public Task<Invitation[]> GetInvitatationsForGame(ObjectId gameId)
        {
            return _invitationStore.GetInvitationsForGame(gameId);
        }

        private async Task<List<Invitation>> _UpdateInvitedPlayers(Game game, Invitation[] invitedPlayers)
        {
            if (invitedPlayers == null)
                return new List<Invitation>();

            var invitations = await _invitationStore.GetInvitationsForGame(game.Id);

            if (invitations == null)
                invitations = new Invitation[0];

            // remove invitations if needed
            foreach (var invitation in invitations)
            {
                var player = invitedPlayers.FirstOrDefault(x => x.Email == invitation.Email);
                if (player == null)
                    await _invitationStore.Remove(invitation);
            }

            // gather info about new invitations
            var newInvitations = new List<Invitation>();
            var invitationsToSend = new List<Invitation>();

            foreach (var invitedPlayer in invitedPlayers)
            {
                var invitation = invitations.FirstOrDefault(x => x.Id == invitedPlayer.Id);
                var invitationMail = invitations.FirstOrDefault(x => x.Email == invitedPlayer.Email);

                if (invitationMail != null && invitation == null)
                {
                    invitationsToSend.Add(invitationMail);
                }
                else if (invitation == null)
                {
                    var item = new Invitation
                    {
                        Email = invitedPlayer.Email,
                        UserId = invitedPlayer.UserId,
                        GameId = game.Id,
                    };

                    newInvitations.Add(item);
                    invitationsToSend.Add(item);
                }
            }

            await _invitationStore.Add(newInvitations.ToArray());

            return invitationsToSend;
        }

        private void _SendInvitationsAsync(Game game, List<Invitation> invitaions)
        {
            Task.Run(() =>
            {
                foreach (var invitation in invitaions)
                {
                    try
                    {
                        _logger.LogInformation($"Sending invitation mail to '{invitation.Email}' ...");
                        _SendInvitation(game, invitation);
                        _logger.LogInformation("Invitation successfully sent.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to send invitation:\r\n{ex.Message}\r\n{ex.StackTrace}");
                    }
                }
            });
        }

        private void _SendInvitation(Game game, Invitation invitation)
        {
            var link = string.Format(LINK, _hostName, invitation.Id);

            var message = new MailMessage
            {
                Body = string.Format(MESSAGE_BODY, game.Title, link),
                Subject = string.Format(MESSAGE_SUBJECT, game.Title)
            };

            message.To.Add(invitation.Email);
            message.From = new MailAddress("invitation@tipexpert.net");

            using (var client = new SmtpClient())
            {
                client.Host = _smtpHost;
                client.Port = _smtpPort;
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_userName, _password);

                client.Send(message);
            };
        }

        private const string LINK = "{0}/invitation/{1}";
        private const string MESSAGE_SUBJECT = "Someone invited you to '{0}'";
        private const string MESSAGE_BODY = "Hello my friend,\r\n\r\n" +
                                             "Someone invited you to the tip game '{0}' at TipExpert.Net :-)\r\n\r\n" +
                                             "Check it out at {1} !\r\n\r\n" +
                                             "Have fun!";
    }
}