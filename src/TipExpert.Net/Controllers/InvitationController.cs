using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using TipExpert.Core.Invitation;

namespace TipExpert.Net.Controllers
{
    [Route("api/invitation")]
    public class InvitationController : Controller
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost("accept")]
        public async Task<IActionResult> Post([FromBody]string token)
        {
            var userId = User.GetUserIdAsObjectId();
            await _invitationService.UpdateInvitationForPlayer(token, userId);

            return Json(new {success = true});
        }
    }
}
