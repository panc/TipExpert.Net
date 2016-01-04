using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public class MatchFinalizationStrategy : IMatchFinalizationStrategy
    {
        private readonly IGameStore _gameStore;

        public MatchFinalizationStrategy(IGameStore gameStore)
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
                
            }
            else
            {
                
            }
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