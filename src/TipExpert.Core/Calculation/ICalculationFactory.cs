namespace TipExpert.Core.Calculation
{
    public interface ICalculationFactory
    {
        IPointsCalculationStrategy GetPointsCalculationStrategy(Game game);

        IProfitCalculationStrategy GetProfitCalculationStrategy(Game game);
    }
}