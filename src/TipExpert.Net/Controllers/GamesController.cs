using System;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using TipExpert.Core;
using TipExpert.Net.Models;

namespace TipExpert.Net.Controllers
{
    [Route("api/games")]
    public class GamesController : Controller
    {
        private readonly IGameStore _gameStore;

        public GamesController(IGameStore gameStore)
        {
            _gameStore = gameStore;
        }

        [HttpGet("{userId}/created")]
        public async Task<GameDto[]> GetCreatedGames(Guid userId)
        {
            var games = await _gameStore.GetAll();
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpGet("{userId}/finished")]
        public async Task<GameDto[]> GetFinishedGames(Guid userId)
        {
            var games = await _gameStore.GetAll();
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpPost()]
        public async Task<GameDto> Post([FromBody]GameDto newGame)
        {
            var game = new Game { Title = newGame.title };
            await _gameStore.Add(game);
            await _gameStore.SaveChangesAsync();

            return Mapper.Map<GameDto>(game);
        }
    }
}
