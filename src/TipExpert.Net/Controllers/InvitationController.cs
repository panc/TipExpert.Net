using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using TipExpert.Core;
using TipExpert.Core.PlayerInvitation;
using TipExpert.Net.Models;

namespace TipExpert.Net.Controllers
{
    [Route("api/invitation")]
    public class InvitationController : Controller
    {
        private readonly IPlayerInvitationService _playerInvitationService;
        private readonly IInvitationStore _invitationStore;

        public InvitationController(IPlayerInvitationService playerInvitationService, IInvitationStore invitationStore)
        {
            _playerInvitationService = playerInvitationService;
            _invitationStore = invitationStore;
        }

        [HttpGet("{token}")]
        public async Task<InvitationDto> GetDetails(string token)
        {
            var invitationId = token.ToObjectId();
            var invitation = await _invitationStore.GetById(invitationId);

            // TODO
            // check if user is correct

            return Mapper.Map<InvitationDto>(invitation);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> Post([FromBody]string token)
        {
            var userId = User.GetUserIdAsObjectId();
            await _playerInvitationService.UpdateInvitationForPlayer(token, userId);

            return Json(new { success = true });
        }
    }
}
