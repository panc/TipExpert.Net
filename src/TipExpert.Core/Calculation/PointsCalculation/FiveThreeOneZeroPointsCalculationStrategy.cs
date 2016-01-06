using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public class FiveThreeOneZeroPointsCalculationStrategy : IPointsCalculationStrategy
    {
        public Task<int> CalculatePoints(Tip tip, Match match)
        {
            return Task.Run(() =>
            {
                var diffMatch = match.HomeScore - match.GuestScore;
                var diffTip = tip.HomeScore - tip.GuestScore;

                if (match.HomeScore == tip.HomeScore && match.GuestScore == tip.GuestScore)
                    return 5;

                if ((diffMatch < 0 && diffTip < 0) || (diffMatch > 0 && diffTip > 0) || (diffMatch == 0 && diffTip == 0))
                    return (diffMatch == diffTip) ? 3 : 1;

                return 0;
            });
        }
    }
}