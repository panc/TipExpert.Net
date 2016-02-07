using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TipExpert.Core;
using TipExpert.Core.PlayerInvitation;
using TipExpert.Net.Models;

namespace TipExpert.Net.Controllers
{
    [Route("api/invitation")]
    public class InvitationController : Controller
    {
        private readonly IPlayerInvitationService _playerInvitationService;
        private readonly ILogger<InvitationController> _logger;

        public InvitationController(IPlayerInvitationService playerInvitationService, ILogger<InvitationController> logger)
        {
            _playerInvitationService = playerInvitationService;
            _logger = logger;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetDetails(string token)
        {
            var invitation = await _playerInvitationService.GetInvitatationsForToken(token);

            IActionResult errorResult =
                _CheckInvitationIsNotNull(invitation, token) ??
                _CheckGameIsNotNull(invitation);

            if (errorResult != null)
                return errorResult;

            return Json(Mapper.Map<InvitationDto>(invitation));
        }

        [HttpPost("accept")]
        public async Task<IActionResult> Post([FromBody]string token)
        {
            var userId = User.GetUserIdAsObjectId();
            var invitation = await _playerInvitationService.GetInvitatationsForToken(token);

            IActionResult errorResult =
                _CheckInvitationIsNotNull(invitation, token) ??
                _CheckGameIsNotNull(invitation);

            if (errorResult != null)
                return errorResult;
            
            await _playerInvitationService.AcceptInvitation(invitation, userId);

            return Json(new { gameId = invitation.GameId });
        }

        private IActionResult _CheckInvitationIsNotNull(Invitation invitation, string token)
        {
            if (invitation != null)
                return null;

            var msg = $"Invitation for token '{token}' not found!";
            _logger.LogError(msg);

            return HttpBadRequest(msg);
        }

        private IActionResult _CheckGameIsNotNull(Invitation invitation)
        {
            if (invitation.Game != null)
                return null;

            var msg = $"Game with id '{invitation.GameId}' not found - but it is defined in invitation '{invitation.Id}'!";
            _logger.LogError(msg);

            return HttpBadRequest(msg);
        }
    }
}
