using System;
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
        private readonly string _smtpHost;
        private readonly int _smtpPort;

        public PlayerInvitationService(IGameStore gameStore, IInvitationStore invitationStore, string userName, string password, string smtpHost, int smtpPort)
        {
            _gameStore = gameStore;
            _invitationStore = invitationStore;
            _userName = userName;
            _password = password;
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
        }

        public void SendInvitationsAsync(Game game, Invitation[] invitedPlayers)
        {
            Task.Run(async () =>
            {
                await _UpdateInvitedPlayers(game, invitedPlayers);
            });
        }

        public async Task UpdateInvitationForPlayer(string token, ObjectId userId)
        {
            // the token is the id of the invitation
            var invitationId = token.ToObjectId();
            var invitation = await _invitationStore.GetById(invitationId);

            if (invitation == null)
            {
                // TODO: Proper error handling
                throw new NotImplementedException("Invitation not found!");
            }

            var game = await _gameStore.GetById(invitation.GameId);

            if (game == null)
            {
                // TODO: Proper error handling
                throw new NotImplementedException("Game for invitation not found!");
            }

            // add player to game
            var player = new Player {UserId = userId};
            game.Players.Add(player);
            
            await _gameStore.Update(game);

            // remove invitation
            await _invitationStore.Remove(invitation);
        }

        public Task<Invitation[]> GetInvitatedPlayersForGame(ObjectId gameId)
        {
            return _invitationStore.GetInvitationsForGame(gameId);
        }

        private async Task _UpdateInvitedPlayers(Game game, Invitation[] invitedPlayers)
        {
            var invitaions = await _invitationStore.GetInvitationsForGame(game.Id);

            // remove invitations if needed
            foreach (var invitation in invitaions)
            {
                var player = invitedPlayers.FirstOrDefault(x => x.Email == invitation.Email);
                if (player == null)
                    await _invitationStore.Remove(invitation);
            }

            // add new invitations and send invitaiton mail
            foreach (var invitedPlayer in invitedPlayers)
            {
                var email = invitedPlayer.Email;
                var invitation = invitaions.FirstOrDefault(x => x.Email == email);

                if (invitation == null)
                {
                    invitation = new Invitation
                    {
                        Email = invitedPlayer.Email,
                        UserId = invitedPlayer.UserId,
                        GameId = game.Id
                    };

                    await _invitationStore.Add(invitation);
                    await _SendInvitation(game, invitation);
                }
            }
        }

        private async Task _SendInvitation(Game game, Invitation invitation)
        {
            var message = new MailMessage();
            message.Body = string.Format(MESSAGE_BODY, invitation.Id);
            message.Subject = MESSAGE_SUBJECT;
            message.To.Add(invitation.Email);
            message.From = new MailAddress("invitation@tipexpert.net");

            var client = new SmtpClient
            {
                Host = _smtpHost,
                Port = _smtpPort,
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