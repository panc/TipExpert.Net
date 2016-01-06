using System;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using TipExpert.Core;
using TipExpert.Core.Strategy;
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

        [HttpGet]
        public async Task<MatchDto[]> Get()
        {
            var leagues = await _matchStore.GetAll();
            return Mapper.Map<MatchDto[]>(leagues);
        }

        [HttpPost]
        public async Task<MatchDto> Post([FromBody]MatchDto newMatch)
        {
            var match = Mapper.Map<Match>(newMatch);
            await _matchStore.Add(match);
            await _matchStore.SaveChangesAsync();

            return Mapper.Map<MatchDto>(match);
        }

        [HttpPut("{id}")]
        public async Task<MatchDto> Put(Guid id, [FromBody]MatchDto matchDto)
        {
            var match = await _matchStore.GetById(id);
            match.DueDate = matchDto.dueDate;
            match.GuestScore = matchDto.guestScore;
            match.HomeScore = matchDto.homeScore;
            match.GuestTeam = matchDto.guestTeam;
            match.HomeTeam = matchDto.homeTeam;
            match.LeagueId = matchDto.leagueId;
            match.IsFinished = matchDto.isFinished;

            await _gameTipsUpdateManager.UpdateGamesForMatch(match);

            await _matchStore.SaveChangesAsync();

            return Mapper.Map<MatchDto>(match);
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            var match = await _matchStore.GetById(id);
            await _matchStore.Remove(match);
            await _matchStore.SaveChangesAsync();
        }
    }
}
