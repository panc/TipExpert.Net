using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> GetGameForEdit(Guid gameId)
        {
            var game = await _gameStore.GetById(gameId);

            if (!_IsCurrentUserGameCreator(game))
                return HttpBadRequest("Only the game creator can edit the game!");
            
            return Json(Mapper.Map<GameDto>(game));
        }

        [HttpGet("created")]
        public async Task<GameDto[]> GetCreatedGames()
        {
            var userId = User.GetUserIdAsGuid();

            var games = await _gameStore.GetGamesCreatedByUser(userId);
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpPost]
        public async Task<GameDto> CreateGame([FromBody]GameDto newGame)
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

        [HttpPut("{gameId}/data")]
        public async Task<IActionResult> UpdateGame(Guid gameId, [FromBody]GameDto gameDto)
        {
            var game = await _gameStore.GetById(gameId);

            if (!_IsCurrentUserGameCreator(game))
                return HttpBadRequest("Only the game creator can edit the game!");
                
            game.Title = gameDto.title;
            game.Description = gameDto.description;
            game.MinStake = gameDto.minStake;

            await _gameStore.SaveChangesAsync();

            return Json(Mapper.Map<GameDto>(game));
        }

        [HttpPut("{gameId}/players")]
        public async Task<IActionResult> UpdatePlayers(Guid gameId, [FromBody]PlayerDto[] playerDtos)
        {
            var game = await _gameStore.GetById(gameId);

            if (!_IsCurrentUserGameCreator(game))
                return HttpBadRequest("Not allowed to edit game!");

            game.Players = Mapper.Map<Player[]>(playerDtos).ToList();

            // game creator always has to be part of the players list
            var creatorId = User.GetUserIdAsGuid();
            if (game.Players.FirstOrDefault(x => x.UserId == creatorId) == null)
            {
                game.Players.Add(new Player { UserId = creatorId });
            }

            await _gameStore.SaveChangesAsync();

            return Json(Mapper.Map<GameDto>(game));
        }

        [HttpPut("{gameId}/matches")]
        public async Task<IActionResult> UpdateMatches(Guid gameId, [FromBody]MatchTipsDto[] matchDtos)
        {
            var game = await _gameStore.GetById(gameId);

            if (!_IsCurrentUserGameCreator(game))
                return HttpBadRequest("Only the game creator can edit the game!");
                
            var ids = matchDtos.Select(x => x.matchId).ToList();
            var list = new List<MatchTips>();

            foreach (var id in ids)
            {
                MatchTips entry = null;

                if (game.Matches != null)
                    entry = game.Matches.FirstOrDefault(x => x.MatchId == id);

                if (entry == null)
                    entry = new MatchTips { MatchId = id };

                list.Add(entry);
            }

            game.Matches = list;
            await _gameStore.SaveChangesAsync();

            return Json(Mapper.Map<GameDto>(game));
        }

        [HttpDelete("{gameId}")]
        public async Task<IActionResult> DeleteGame(Guid gameId)
        {
            var game = await _gameStore.GetById(gameId);

            if (!_IsCurrentUserGameCreator(game))
                return HttpBadRequest("Only the game creator can edit the game!");

            if (game.IsFinished)
                return HttpBadRequest("Can not delete a finished game!");

            // todo: check whether the game is already running

            await _gameStore.Remove(game);
            await _gameStore.SaveChangesAsync();

            return Json(new { success = true });
        }

        private bool _IsCurrentUserGameCreator(Game game)
        {
            var userId = User.GetUserIdAsGuid();
            return game.CreatorId == userId;
        }
    }
}