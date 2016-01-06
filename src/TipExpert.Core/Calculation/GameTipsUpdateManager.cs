using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core.Calculation
{
    public class GameTipsUpdateManager : IGameTipsUpdateManager
    {
        private readonly IGameStore _gameStore;
        private readonly ICalculationResolver _calculationResolver;

        public GameTipsUpdateManager(IGameStore gameStore, ICalculationResolver calculationResolver)
        {
            _gameStore = gameStore;
            _calculationResolver = calculationResolver;
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

            await _UpdateTipsForMatch(game, match);
            _UpdateTotalPoints(game);
            _UpdateRanking(game);
            await _UpdateProfit(game);
        }

        private async Task _UpdateTipsForMatch(Game game, Match match)
        {
            var mt = game.Matches.FirstOrDefault(x => x.MatchId == match.Id);

            if (mt == null)
                return;

            var pointsCalculationStrategy = _calculationResolver.GetPointsCalculationStrategy(game);

            foreach (var tip in mt.Tips)
            {
                if (match.IsFinished)
                    tip.Points = await pointsCalculationStrategy.CalculatePoints(tip, match);
                else
                    tip.Points = null;
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
                player.TotalPoints = pointsForUser[player.UserId];
        }

        private void _UpdateRanking(Game game)
        {
            var players = game.Players.OrderByDescending(x => x.TotalPoints).ToArray();

            for (int i = 1; i <= players.Length; i++)
            {
                var player = players[i - 1];
                player.Ranking = i;
            }
        }

        private async Task _UpdateProfit(Game game)
        {
            // calulate profit for each user
            var profitCalcualationStrategy = _calculationResolver.GetProfitCalculationStrategy(game);

            if (game.IsFinished)
                await profitCalcualationStrategy.CalcualteProfit(game);
            else
                await profitCalcualationStrategy.ResetProfit(game);
        }
    }
}