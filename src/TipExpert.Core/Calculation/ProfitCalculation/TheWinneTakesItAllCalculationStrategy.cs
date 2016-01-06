using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core.Calculation
{
    public class TheWinneTakesItAllCalculationStrategy : IProfitCalculationStrategy
    {
        private readonly IUserStore _userStore;

        public TheWinneTakesItAllCalculationStrategy(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public async Task CalcualteProfit(Game game)
        {
            var players = game.Players.OrderBy(x => x.Ranking);

            var totalStake = players.Sum(x => x.Stake.GetValueOrDefault(game.MinStake));
            var winner = players.FirstOrDefault();

            foreach (var player in players)
            {
                player.Profit = player == winner ? totalStake : 0;

                // update the users coins
                var user = await _userStore.GetById(player.UserId);
                user.Coins += player.Profit.GetValueOrDefault(0);
                await _userStore.SaveChangesAsync();
            }
        }

        public async Task ResetProfit(Game game)
        {
            foreach (var player in game.Players)
            {
                var user = await _userStore.GetById(player.UserId);
                user.Coins -= player.Profit.GetValueOrDefault(0);
                await _userStore.SaveChangesAsync();

                player.Profit = 0;
            }
        }
    }
}