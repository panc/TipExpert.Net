using System.Threading.Tasks;

namespace TipExpert.Core.Calculation
{
    public interface IPointsCalculationStrategy
    {
        Task<int> CalculatePoints(Tip tip, Match match);
    }
}