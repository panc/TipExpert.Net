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
            await _gameStore.Update(game);

            return Json(_PrepareGameForUser(game));
        }

        [HttpPut("{gameId}/tip")]
        public async Task<IActionResult> UpdateTip(Guid gameId, [FromBody]MatchTipsDto matchTipDto)
        {
            var game = await _gameStore.GetById(gameId);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, gameId) ??
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

            var userId = User.GetUserIdAsGuid();
            var match = game.Matches.FirstOrDefault(x => x.MatchId == matchTipDto.matchId);

            if (match == null)
                return HttpBadRequest("The match is not defined for that game!");

            if (match.Tips == null)
                match.Tips = new List<Tip>();

            var tip = match.Tips.FirstOrDefault(x => x.UserId == userId);
            if (tip == null)
            {
                tip = new Tip();
                tip.UserId = userId;
                match.Tips.Add(tip);
            }

            tip.HomeScore = matchTipDto.tipOfPlayer.homeScore;
            tip.GuestScore = matchTipDto.tipOfPlayer.guestScore;

            await _gameStore.Update(game);

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

            await _gameStore.Update(game);

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

            await _gameStore.Update(game);

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
                var entry = game.Matches?.FirstOrDefault(x => x.MatchId == id);

                if (entry == null)
                    entry = new MatchTips { MatchId = id };

                list.Add(entry);
            }

            game.Matches = list;
            await _gameStore.Update(game);

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

            return Json(new { success = true });
        }

        private GameDto _PrepareGameForUser(Game game)
        {
            var userId = User.GetUserIdAsGuid();
            var gameDto = Mapper.Map<GameDto>(game);

            gameDto.player = gameDto.players?.FirstOrDefault(x => x.userId == userId);

            if (gameDto.matches != null)
            {
                gameDto.finishedMatches = _GetFinishedMatches(gameDto).ToList();
                gameDto.matches = _GetNotFinishedMatches(gameDto, userId).ToList();
            }

            return gameDto;
        }

        private IEnumerable<MatchTipsDto> _GetFinishedMatches(GameDto gameDto)
        {
            var finishedMatches = gameDto.matches.Where(x => x.match != null && x.match.isFinished);

            foreach (var mt in finishedMatches)
            {
                // merge tips with the ranking of the user
                var mergedTips = mt.tips
                    .Select(t => new
                    {
                        Tip = t,
                        Ranking = gameDto.players.FindIndex(p => p.userId == t.userId)
                    });

                // order tipps regarding the ranking of the user
                var orderedTips = mergedTips.OrderBy(x => x.Ranking);
                mt.tips = orderedTips.Select(x => x.Tip).ToList();

                yield return mt;
            }
        }

        private IEnumerable<MatchTipsDto> _GetNotFinishedMatches(GameDto gameDto, Guid userId)
        {
            foreach (var match in gameDto.matches.Where(x => x.match != null && !x.match.isFinished))
            {
                match.tipOfPlayer = match.tips?.FirstOrDefault(x => x.userId == userId);
                match.tips = null; // do not pass all tips to the client so that others can not see the tips of all players...

                yield return match;
            }
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