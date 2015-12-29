using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using TipExpert.Core;

namespace TipExpert.Net.Controllers
{
    [Route("api/leagues")]
    public class LeaguesController : Controller
    {
        private readonly ILeagueStore _leagueStore;

        public LeaguesController(ILeagueStore leagueStore)
        {
            _leagueStore = leagueStore;
        }

        [HttpGet]
        public async Task<League[]> Get()
        {
            return await _leagueStore.GetAll();
        }
    }
}
