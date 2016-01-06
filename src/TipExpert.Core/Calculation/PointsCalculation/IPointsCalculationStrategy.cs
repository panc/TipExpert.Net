using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public interface IPointsCalculationStrategy
    {
        Task<int> CalculatePoints(Tip tip, Match match);
    }
}