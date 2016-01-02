using System;
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
        public async Task<MatchDto[]> GetMatches(Guid leagueId)
        {
            var matches = await _matchStore.GetMatchesForLeague(leagueId);
            return Mapper.Map<MatchDto[]>(matches);
        }

        [HttpGet("{leagueId}/matches/notfinished")]
        public async Task<MatchDto[]> GetNotFinishedMatches(Guid leagueId)
        {
            var matches = await _matchStore.GetMatchesForLeague(leagueId);

            return Mapper.Map<MatchDto[]>(matches.Where(x => !x.IsFinished));
        }

        [HttpPost]
        public async Task<LeagueDto> Post([FromBody]LeagueDto newLeague)
        {
            var league = Mapper.Map<League>(newLeague);
            await _leagueStore.Add(league);
            await _leagueStore.SaveChangesAsync();

            return Mapper.Map<LeagueDto>(league);
        }

        [HttpPut("{id}")]
        public async Task<LeagueDto> Put(Guid id, [FromBody]LeagueDto leagueDto)
        {
            var league = await _leagueStore.GetById(id);
            league.Name = leagueDto.name;

            await _leagueStore.SaveChangesAsync();

            return Mapper.Map<LeagueDto>(league);
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            var league = await _leagueStore.GetById(id);
            await _leagueStore.Remove(league);
            await _leagueStore.SaveChangesAsync();
        }
    }
}
