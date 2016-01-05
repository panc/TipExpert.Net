using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public interface IProfitCalculationStrategy
    {
        Task CalcualteProfit(Game game);

        Task ResetProfit(Game game);
    }
}