using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using TipExpert.Core;
using TipExpert.Core.Calculation;
using TipExpert.Net.Models;

namespace TipExpert.Net.Controllers
{
    [Route("api/matches")]
    public class MatchesController : Controller
    {
        private readonly IMatchStore _matchStore;
        private readonly IGameTipsUpdateManager _gameTipsUpdateManager;

        public MatchesController(IMatchStore matchStore, IGameTipsUpdateManager gameTipsUpdateManager)
        {
            _matchStore = matchStore;
            _gameTipsUpdateManager = gameTipsUpdateManager;
        }

        [HttpPost]
        public async Task<MatchDto> Post([FromBody]MatchDto newMatch)
        {
            var match = Mapper.Map<Match>(newMatch);
            await _matchStore.Add(match);
            
            return Mapper.Map<MatchDto>(match);
        }

        [HttpPut("{matchId}")]
        public async Task<MatchDto> Put(string matchId, [FromBody]MatchDto matchDto)
        {
            var id = matchId.ToObjectId();
            var match = await _matchStore.GetById(id);
            match.DueDate = matchDto.dueDate;
            match.GuestScore = matchDto.guestScore;
            match.HomeScore = matchDto.homeScore;
            match.GuestTeam = matchDto.guestTeam;
            match.HomeTeam = matchDto.homeTeam;
            match.IsFinished = matchDto.isFinished;
            match.LeagueId = matchDto.leagueId.ToObjectId();

            // Todo: Error handing
            await _matchStore.Update(match);
            await _gameTipsUpdateManager.UpdateGamesForMatch(match);

            return Mapper.Map<MatchDto>(match);
        }

        [HttpDelete("{matchId}")]
        public async Task Delete(string matchId)
        {
            var id = matchId.ToObjectId();
            var match = await _matchStore.GetById(id);
            await _matchStore.Remove(match);
        }
    }
}
