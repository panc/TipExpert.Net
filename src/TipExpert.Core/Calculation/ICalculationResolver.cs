namespace TipExpert.Core.Calculation
{
    public interface ICalculationResolver
    {
        IPointsCalculationStrategy GetPointsCalculationStrategy(Game game);

        IProfitCalculationStrategy GetProfitCalculationStrategy(Game game);
    }
}