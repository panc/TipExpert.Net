﻿using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using TipExpert.Core;
using TipExpert.Net.Models;

namespace TipExpert.Net.Controllers
{
    [Route("api/matches")]
    public class MatchesController : Controller
    {
        private readonly IMatchStore _matchStore;

        public MatchesController(IMatchStore matchStore)
        {
            _matchStore = matchStore;
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
    }
}
