using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using TipExpert.Core.Invitation;

namespace TipExpert.Net.Controllers
{
    [Route("api/invitation")]
    public class InvitationController : Controller
    {
        private readonly IInvitationTokeService _tokeService;

        public InvitationController(IInvitationTokeService tokeService)
        {
            _tokeService = tokeService;
        }

        [HttpPost("accept")]
        public async Task<IActionResult> Post([FromBody]string token)
        {
            var userId = User.GetUserIdAsObjectId();
            await _tokeService.UpdatePlayerForToken(token, userId);

            return Json(new {success = true});
        }
    }
}
