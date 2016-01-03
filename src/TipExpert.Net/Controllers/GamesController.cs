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
        public async Task<IActionResult> GetGameForUser(Guid gameId)
        {
            var game = await _gameStore.GetById(gameId);

            IActionResult errorResult = _CheckGameIsNotNull(game, gameId);
            if (errorResult != null)
                return errorResult;

            return Json(_PrepareGameForUser(game));
        }

        [HttpGet("{gameId}/edit")]
        public async Task<IActionResult> GetGameForEdit(Guid gameId)
        {
            var game = await _gameStore.GetById(gameId);

            IActionResult errorResult = _CheckGameIsNotNull(game, gameId);
            if (errorResult != null)
                return errorResult;
            
            return Json(Mapper.Map<GameDto>(game));
        }

        [HttpGet("created")]
        public async Task<GameDto[]> GetCreatedGames()
        {
            var userId = User.GetUserIdAsGuid();

            var games = await _gameStore.GetGamesCreatedByUser(userId);
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpGet("invited")]
        public async Task<GameDto[]> GetInvitedGames()
        {
            var userId = User.GetUserIdAsGuid();

            var games = await _gameStore.GetGamesUserIsInvitedTo(userId);
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpGet("finished")]
        public async Task<GameDto[]> GetFinishedGames()
        {
            var userId = User.GetUserIdAsGuid();

            var games = await _gameStore.GetFinishedGames(userId);
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

        [HttpPut("{gameId}/stake")]
        public async Task<IActionResult> UpdateStake(Guid gameId, [FromBody]int stake)
        {
            var game = await _gameStore.GetById(gameId);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, gameId) ??
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

            var userId = User.GetUserIdAsGuid();
            var player = game.Players.FirstOrDefault(x => x.UserId == userId);

            if (player == null)
                return HttpBadRequest("The user is not defined as a player of that game!");

            player.Stake = stake;
            await _gameStore.SaveChangesAsync();

            return Json(_PrepareGameForUser(game));
        }

        [HttpPut("{gameId}/edit/data")]
        public async Task<IActionResult> UpdateGame(Guid gameId, [FromBody]GameDto gameDto)
        {
            var game = await _gameStore.GetById(gameId);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, gameId) ??
                _CheckCurrentUserIsGameCreator(game) ??
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

            game.Title = gameDto.title;
            game.Description = gameDto.description;
            game.MinStake = gameDto.minStake;

            await _gameStore.SaveChangesAsync();

            return Json(Mapper.Map<GameDto>(game));
        }

        [HttpPut("{gameId}/edit/players")]
        public async Task<IActionResult> UpdatePlayers(Guid gameId, [FromBody]PlayerDto[] playerDtos)
        {
            var game = await _gameStore.GetById(gameId);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, gameId) ??
                _CheckCurrentUserIsGameCreator(game) ??
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

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

        [HttpPut("{gameId}/edit/matches")]
        public async Task<IActionResult> UpdateMatches(Guid gameId, [FromBody]MatchTipsDto[] matchDtos)
        {
            var game = await _gameStore.GetById(gameId);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, gameId) ??
                _CheckCurrentUserIsGameCreator(game) ??
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;
                
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

        [HttpDelete("{gameId}/edit")]
        public async Task<IActionResult> DeleteGame(Guid gameId)
        {
            var game = await _gameStore.GetById(gameId);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, gameId) ??
                _CheckCurrentUserIsGameCreator(game) ?? 
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

            // todo: check whether the game is already running

            await _gameStore.Remove(game);
            await _gameStore.SaveChangesAsync();

            return Json(new { success = true });
        }

        private GameDto _PrepareGameForUser(Game game)
        {
            var userId = User.GetUserIdAsGuid();
            var gameDto = Mapper.Map<GameDto>(game);

            gameDto.player = gameDto.players.FirstOrDefault(x => x.userId == userId);

            return gameDto;
        }

        private IActionResult _CheckCurrentUserIsGameCreator(Game game)
        {
            var userId = User.GetUserIdAsGuid();

            return game.CreatorId == userId
                ? null
                : HttpBadRequest("Only the game creator can edit the game!");
        }

        private IActionResult _CheckGameIsNotFinished(Game game)
        {
            return (game.IsFinished)
                ? HttpBadRequest("Can not change a finished game!")
                : null;
        }

        private IActionResult _CheckGameIsNotNull(Game game, Guid gameId)
        {
            return (game == null)
                ? HttpBadRequest($"Game with id '{gameId}' not found!")
                : null;
        }
    }
}