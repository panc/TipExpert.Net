using System.Linq;

namespace TipExpert.Core.Strategy
{
    public class TheWinneTakesItAllCalculationStrategy : IProfitCalculationStrategy
    {
        public void CalcualteProfit(Game game)
        {
            var totalStake = game.Players.Sum(x => x.Stake.GetValueOrDefault(game.MinStake));

            game.Players.Sort((a, b) =>
            {
                var pointsA = a.TotalPoints.GetValueOrDefault(0);
                var pointsB = b.TotalPoints.GetValueOrDefault(0);

                if (a == b)
                    return 0;

                return pointsB - pointsA;
            });



            /*
             
            var winner = players[0];

            players.forEach( function ( player ) {

                if ( player == winner )
                    player.profit = totalStake;
                else
                    player.profit = 0;

                // update the users coins
                var userModel = mongoose.model( 'User' );
                userModel.load( player.user, function ( err, user ) {
                    if ( err )
                        return;

                    user.coins += player.profit;
                    user.save();
                    logger.debug( 'user: ' + user.name + ' - cash: ' + user.coins );
                });

                logger.debug( 'player: ' + player.user + ' - profit: ' + player.profit );
            });
            */
        }

        public void ResetProfit(Game game)
        {
            throw new System.NotImplementedException();
        }
    }
}