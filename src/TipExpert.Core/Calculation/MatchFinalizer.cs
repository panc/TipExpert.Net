using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public class MatchFinalizer : IMatchFinalizer
    {
        private readonly IGameStore _gameStore;

        public MatchFinalizer(IGameStore gameStore)
        {
            _gameStore = gameStore;
        }

        public async Task UpdateGamesForMatch(Match match)
        {
            var games = await _gameStore.GetGamesForMatch(match.Id);

            foreach (var game in games)
            {
                _UpdateTipsOfGame(game, match);
                _FinishGameAndUpdateTotalPoints(game);
            }

            await _gameStore.SaveChangesAsync();
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

        private void _FinishGameAndUpdateTotalPoints(Game game)
        {
            var pointsForUser = new Dictionary<Guid, int>();
            var allMatchesFinished = true;

            foreach (var mt in game.Matches)
            {
                allMatchesFinished = mt.Match.IsFinished;
                if (allMatchesFinished == false)
                    break;

                foreach (var tip in mt.Tips)
                    pointsForUser[tip.UserId] += tip.Points.GetValueOrDefault(0);
            }

            // update total points for all players
            game.IsFinished = allMatchesFinished;

            if (allMatchesFinished)
            {
                foreach (var player in game.Players)
                    player.TotalPoints = pointsForUser[player.UserId];
            }

            // only 'The winner gets it all' mode is currently supported
            var profitCalcualationStrategy = new TheWinneTakesItAllCalculationStrategy();

            if (allMatchesFinished)
                profitCalcualationStrategy.CalcualteProfit(game);
            else
                profitCalcualationStrategy.ResetProfit(game);
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

        /*
        
        var setProfit = function (players, userPoints, minStake) {

            var totalStake = 0;

            players.forEach( function ( player ) {
                logger.debug( 'stake: ' + player.stake );
                totalStake += player.stake || minStake;

                var totalPoints = userPoints[player.user];
                player.totalPoints = totalPoints;
            });

            players.sort( comparePlayer );

            // only 'The winner gets it all' mode is currently supported
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
        };

        var resetProfit = function ( players ) {

            players.forEach( function ( player ) {
                var oldProfit = player.profit;
                player.totalPoints = null;
                player.profit = null;

                if ( oldProfit == 0 )
                    return;

                // reset the users coins
                var userModel = mongoose.model( 'User' );
                userModel.load( player.user, function ( err, user ) {
                    if ( err )
                        return;

                    user.coins -= oldProfit;
                    user.save();
                    logger.debug( 'user: ' + user.name + ' - cash: ' + user.coins );
                });
            });
        }

        var comparePlayer = function ( a, b ) {
            if ( !a.totalPoints && !b.totalPoints || a == b )
                return 0;
            if ( !a.totalPoints )
                return 1;
            if ( !b.totalPoints )
                return -1;

            return b.totalPoints - a.totalPoints;
        };

        */
    }
}