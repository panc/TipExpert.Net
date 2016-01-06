using System.Threading.Tasks;

namespace TipExpert.Core.Calculation
{
    public interface IProfitCalculationStrategy
    {
        Task CalcualteProfit(Game game);

        Task ResetProfit(Game game);
    }
}