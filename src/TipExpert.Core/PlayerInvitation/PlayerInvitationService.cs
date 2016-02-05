using System;
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
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _smtpHost;
        private readonly int _smtpPort;

        public PlayerInvitationService(IGameStore gameStore, IInvitationStore invitationStore, 
            string hostName, string userName, string password, string smtpHost, int smtpPort)
        {
            _gameStore = gameStore;
            _invitationStore = invitationStore;
            _hostName = hostName;
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
                // todo: check for double email address
                var invitation = invitaions.FirstOrDefault(x => x.Id == invitedPlayer.Id);

                if (invitation == null)
                {
                    invitation = new Invitation
                    {
                        Email = invitedPlayer.Email,
                        UserId = invitedPlayer.UserId,
                        GameId = game.Id,
                        State = InvitationState.SendingMail
                    };

                    // save the first time so that the user can see the current state
                    await _invitationStore.Add(invitation);

                    try
                    {
                        _SendInvitation(game, invitation);
                        invitation.State = InvitationState.Success;
                    }
                    catch (Exception ex)
                    {
                        invitation.Error = $"{ex.Message}\r\n{ex.StackTrace}";
                        invitation.State = InvitationState.Error;
                    }

                    // save again to update the current state.
                    await _invitationStore.Update(invitation);
                }
            }
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

                // TODO
                // Add error handling and show the state of the email-send process in the edit view.

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