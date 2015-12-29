using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using TipExpert.Core;
using TipExpert.Net.Models;

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
        public async Task<LeagueDto[]> Get()
        {
            var leagues = await _leagueStore.GetAll();
            return Mapper.Map<LeagueDto[]>(leagues);
        }
    }
}
