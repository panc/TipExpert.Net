namespace TipExpert.Core.Strategy
{
    public interface ICalculationResolver
    {
        IPointsCalculationStrategy GetPointsCalculationStrategy(Game game);

        IProfitCalculationStrategy GetProfitCalculationStrategy(Game game);
    }
}