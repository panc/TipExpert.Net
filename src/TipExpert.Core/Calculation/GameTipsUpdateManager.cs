using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public class GameTipsUpdateManager : IGameTipsUpdateManager
    {
        private readonly IGameStore _gameStore;
        private readonly IUserStore _userStore;

        public GameTipsUpdateManager(IGameStore gameStore, IUserStore userStore)
        {
            _gameStore = gameStore;
            _userStore = userStore;
        }

        public async Task UpdateGamesForMatch(Match match)
        {
            var games = await _gameStore.GetGamesForMatch(match.Id);

            var tasks = games.Select(game => _UpdateGame(game, match));
            await Task.WhenAll(tasks);

            await _gameStore.SaveChangesAsync();
        }

        private async Task _UpdateGame(Game game, Match match)
        {
            if (game == null || game.Matches == null)
                return;

            game.IsFinished = game.Matches.All(x => x.Match != null && x.Match.IsFinished);

            _UpdateTipsOfGame(game, match);
            _UpdateTotalPoints(game);
            _UpdateRanking(game);
            await _UpdateProfit(game);
        }

        private void _UpdateTipsOfGame(Game game, Match match)
        {
            var mt = game.Matches.FirstOrDefault(x => x.MatchId == match.Id);

            if (mt == null)
                return;

            foreach (var tip in mt.Tips)
            {
                if (match.IsFinished)
                    _SetPoints(tip, match);
                else
                    _ResetPoints(tip);
            }
        }

        private void _UpdateTotalPoints(Game game)
        {
            var pointsForUser = game.Players.ToDictionary(x => x.UserId, x => 0);

            foreach (var mt in game.Matches)
            {
                foreach (var tip in mt.Tips)
                    pointsForUser[tip.UserId] += tip.Points.GetValueOrDefault(0);
            }

            // set (or reset) total points for all players
            foreach (var player in game.Players)
                player.TotalPoints = game.IsFinished ? pointsForUser[player.UserId] : 0;
        }

        private void _UpdateRanking(Game game)
        {
            var players = game.Players.OrderByDescending(x => x.TotalPoints).ToArray();

            for (int i = 1; i < players.Length; i++)
            {
                var player = players[i];
                player.Ranking = i;
            }
        }

        private async Task _UpdateProfit(Game game)
        {
            // calulate profit for each user
            // only 'The winner takes it all' mode is currently supported
            var profitCalcualationStrategy = new TheWinneTakesItAllCalculationStrategy(_userStore);

            if (game.IsFinished)
                await profitCalcualationStrategy.CalcualteProfit(game);
            else
                await profitCalcualationStrategy.ResetProfit(game);
        }

        private void _SetPoints(Tip tip, Match match)
        {
            var diffMatch = match.HomeScore - match.GuestScore;
            var diffTip = tip.HomeScore - tip.GuestScore;

            if (match.HomeScore == tip.HomeScore && match.GuestScore == tip.GuestScore)
                tip.Points = 5;

            else if ((diffMatch < 0 && diffTip < 0) || (diffMatch > 0 && diffTip > 0) || (diffMatch == 0 && diffTip == 0))
                tip.Points = (diffMatch == diffTip) ? 3 : 1;

            else
                tip.Points = 0;
        }

        private void _ResetPoints(Tip tip)
        {
            tip.Points = null;
        }
    }
}