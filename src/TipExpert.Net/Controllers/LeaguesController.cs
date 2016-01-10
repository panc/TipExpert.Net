using System.Linq;
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
        private readonly IMatchStore _matchStore;

        public LeaguesController(ILeagueStore leagueStore, IMatchStore matchStore)
        {
            _leagueStore = leagueStore;
            _matchStore = matchStore;
        }

        [HttpGet]
        public async Task<LeagueDto[]> Get()
        {
            var leagues = await _leagueStore.GetAll();
            return Mapper.Map<LeagueDto[]>(leagues);
        }

        [HttpGet("{leagueId}/matches")]
        public async Task<MatchDto[]> GetMatches(string leagueId)
        {
            var id = leagueId.ToObjectId();
            var matches = await _matchStore.GetMatchesForLeague(id);
            return Mapper.Map<MatchDto[]>(matches);
        }

        [HttpGet("{leagueId}/matches/notfinished")]
        public async Task<MatchDto[]> GetNotFinishedMatches(string leagueId)
        {
            var id = leagueId.ToObjectId();
            var matches = await _matchStore.GetMatchesForLeague(id);

            return Mapper.Map<MatchDto[]>(matches.Where(x => !x.IsFinished));
        }

        [HttpPost]
        public async Task<LeagueDto> Post([FromBody]LeagueDto newLeague)
        {
            var league = Mapper.Map<League>(newLeague);
            await _leagueStore.Add(league);

            return Mapper.Map<LeagueDto>(league);
        }

        [HttpPut("{leagueId}")]
        public async Task<LeagueDto> Put(string leagueId, [FromBody]LeagueDto leagueDto)
        {
            var id = leagueId.ToObjectId();
            var league = await _leagueStore.GetById(id);
            league.Name = leagueDto.name;

            await _leagueStore.Update(league);

            return Mapper.Map<LeagueDto>(league);
        }

        [HttpDelete("{leagueId}")]
        public async Task Delete(string leagueId)
        {
            // todo:
            // what happens with exisiting matches???

            var id = leagueId.ToObjectId();
            var league = await _leagueStore.GetById(id);
            await _leagueStore.Remove(league);
        }
    }
}
