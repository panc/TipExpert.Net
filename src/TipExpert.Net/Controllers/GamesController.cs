using System;
using System.Security.Claims;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using TipExpert.Core;
using TipExpert.Net.Authentication;
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

        [HttpGet("{gameId}")]
        public async Task<GameDto> GetGameForUser(Guid gameId)
        {
            var game = await _gameStore.GetById(gameId);
            return Mapper.Map<GameDto>(game);
        }

        [HttpGet("{gameId}/edit")]
        public async Task<GameDto> GetGameForEdit(Guid gameId)
        {
            var game = await _gameStore.GetById(gameId);
            return Mapper.Map<GameDto>(game);
        }

        [HttpGet("created")]
        public async Task<GameDto[]> GetCreatedGames()
        {
            var userId = User.GetUserIdAsGuid();

            var games = await _gameStore.GetGamesCreatedByUser(userId);
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpPost()]
        public async Task<GameDto> Post([FromBody]GameDto newGame)
        {
            var game = new Game
            {
                Title = newGame.title,
                CreatorId = User.GetUserIdAsGuid()
            };

            await _gameStore.Add(game);
            await _gameStore.SaveChangesAsync();

            return Mapper.Map<GameDto>(game);
        }
    }
}
