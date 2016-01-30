using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Bson;
using TipExpert.Core;
using TipExpert.Core.MatchSelection;
using TipExpert.Net.Models;

namespace TipExpert.Net.Controllers
{
    [Route("api/games")]
    public class GamesController : Controller
    {
        private readonly IGameStore _gameStore;
        private readonly IMatchSelectorFactory _matchSelectorFactory;

        public GamesController(IGameStore gameStore, IMatchSelectorFactory matchSelectorFactory)
        {
            _gameStore = gameStore;
            _matchSelectorFactory = matchSelectorFactory;
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGameForUser(string gameId)
        {
            var id = gameId.ToObjectId();
            var game = await _gameStore.GetById(id);

            IActionResult errorResult = _CheckGameIsNotNull(game, id);
            if (errorResult != null)
                return errorResult;

            return Json(_PrepareGameForUser(game));
        }

        [HttpGet("{gameId}/edit")]
        public async Task<IActionResult> GetGameForEdit(string gameId)
        {
            var id = gameId.ToObjectId();
            var game = await _gameStore.GetById(id);

            IActionResult errorResult = _CheckGameIsNotNull(game, id);
            if (errorResult != null)
                return errorResult;

            return Json(Mapper.Map<GameDto>(game));
        }

        [HttpGet("created")]
        public async Task<GameDto[]> GetCreatedGames()
        {
            var userId = User.GetUserIdAsObjectId();

            var games = await _gameStore.GetGamesCreatedByUser(userId);
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpGet("invited")]
        public async Task<GameDto[]> GetInvitedGames()
        {
            var userId = User.GetUserIdAsObjectId();

            var games = await _gameStore.GetGamesUserIsInvitedTo(userId);
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpGet("finished")]
        public async Task<GameDto[]> GetFinishedGames()
        {
            var userId = User.GetUserIdAsObjectId();

            var games = await _gameStore.GetFinishedGames(userId);
            return Mapper.Map<GameDto[]>(games);
        }

        [HttpPost]
        public async Task<GameDto> CreateGame([FromBody]GameDto newGame)
        {
            var game = new Game
            {
                CreatorId = User.GetUserIdAsObjectId()
            };

            // game creator always has to be part of the players list
            var creatorId = User.GetUserIdAsObjectId();
            game.Players = new List<Player>();
            game.Players.Add(new Player { UserId = creatorId });

            _UpdateGameData(game, newGame);
            _UpdatePlayers(game, newGame);
            await _UpdateMatches(game, newGame);

            await _gameStore.Add(game);

            return Mapper.Map<GameDto>(game);
        }

        [HttpPut("{gameId}/stake")]
        public async Task<IActionResult> UpdateStake(string gameId, [FromBody]int stake)
        {
            var id = gameId.ToObjectId();
            var game = await _gameStore.GetById(id);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, id) ??
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

            var userId = User.GetUserIdAsObjectId();
            var player = game.Players.FirstOrDefault(x => x.UserId == userId);

            if (player == null)
                return HttpBadRequest("The user is not defined as a player of that game!");

            player.Stake = stake;
            await _gameStore.Update(game);

            return Json(_PrepareGameForUser(game));
        }

        [HttpPut("{gameId}/tip")]
        public async Task<IActionResult> UpdateTip(string gameId, [FromBody]MatchTipsDto matchTipDto)
        {
            var id = gameId.ToObjectId();
            var game = await _gameStore.GetById(id);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, id) ??
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

            var userId = User.GetUserIdAsObjectId();
            var matchId = matchTipDto.matchId.ToObjectId();
            var match = game.Matches.FirstOrDefault(x => x.MatchId == matchId);

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
        public async Task<IActionResult> UpdateGame(string gameId, [FromBody]GameDto gameDto)
        {
            var id = gameId.ToObjectId();
            var game = await _gameStore.GetById(id);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, id) ??
                _CheckCurrentUserIsGameCreator(game) ??
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

            _UpdateGameData(game, gameDto);
            _UpdatePlayers(game, gameDto);
            await _UpdateMatches(game, gameDto);

            await _gameStore.Update(game);

            return Json(Mapper.Map<GameDto>(game));
        }

        [HttpDelete("{gameId}/edit")]
        public async Task<IActionResult> DeleteGame(string gameId)
        {
            var id = gameId.ToObjectId();
            var game = await _gameStore.GetById(id);

            IActionResult errorResult =
                _CheckGameIsNotNull(game, id) ??
                _CheckCurrentUserIsGameCreator(game) ?? 
                _CheckGameIsNotFinished(game);

            if (errorResult != null)
                return errorResult;

            // todo: check whether the game is already running

            await _gameStore.Remove(game);

            return Json(new { success = true });
        }

        private static void _UpdateGameData(Game game, GameDto gameDto)
        {
            game.Title = gameDto.title;
            game.Description = gameDto.description;
            game.MinStake = gameDto.minStake;
        }

        private void _UpdatePlayers(Game game, GameDto gameDto)
        {

            var invitedPlayers = new List<InvitedPlayer>();

            foreach (var invitedPlayer in gameDto.invitedPlayers)
            {
                var userId = invitedPlayer.userId.ToObjectId();
                var player = game.InvitedPlayers.FirstOrDefault(x => x.UserId == userId);

                if (player == null)
                    player = new InvitedPlayer();

                invitedPlayers.Add(player);
            }

            game.InvitedPlayers = invitedPlayers;
        }

        private async Task _UpdateMatches(Game game, GameDto gameDto)
        {
            game.MatchesMetadata = gameDto.matchesMetadata;
            game.MatchSelectionMode = _MapMatchSelectionMode(gameDto.matchSelectionMode);

            var matchSelector = _matchSelectorFactory.GetMatchSelector(game.MatchSelectionMode);
            var newMatches = await matchSelector.GetMatches(game.MatchesMetadata);

            var list = new List<MatchTips>();

            foreach (var match in newMatches)
            {
                var entry = game.Matches?.FirstOrDefault(x => x.MatchId == match.Id);

                if (entry == null)
                    entry = new MatchTips { MatchId = match.Id };

                list.Add(entry);
            }

            game.Matches = list;
        }

        private MatchSelectionMode _MapMatchSelectionMode(string matchSelectionMode)
        {
            switch (matchSelectionMode)
            {
                case "em2016":
                    return MatchSelectionMode.EM2016;

                case "league":
                    return MatchSelectionMode.League;

                default:
                    throw new NotSupportedException(string.Format("MatchSelectionMode '{0}' is not supported!", matchSelectionMode));
            }
        }

        private GameDto _PrepareGameForUser(Game game)
        {
            var userId = User.GetUserId();
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

        private IEnumerable<MatchTipsDto> _GetNotFinishedMatches(GameDto gameDto, string userId)
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
            var userId = User.GetUserIdAsObjectId();

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

        private IActionResult _CheckGameIsNotNull(Game game, ObjectId gameId)
        {
            return (game == null)
                ? HttpBadRequest($"Game with id '{gameId}' not found!")
                : null;
        }
    }
}