using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
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
            if (game.Players == null)
                return;

            game.SortPlayers();

            var totalStake = game.Players.Sum(x => x.Stake.GetValueOrDefault(game.MinStake));
            var winner = game.Players.FirstOrDefault();

            foreach (var player in game.Players)
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