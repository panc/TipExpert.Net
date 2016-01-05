namespace TipExpert.Core.Strategy
{
    public interface IProfitCalculationStrategy
    {
        void CalcualteProfit(Game game);

        void ResetProfit(Game game);
    }
}