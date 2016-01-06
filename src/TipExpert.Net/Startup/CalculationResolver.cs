using System;
using TipExpert.Core.Calculation;

namespace TipExpert.Core.Configuration
{
    public class CalculationResolver : ICalculationResolver
    {
        private readonly IServiceProvider _hostingEnvironment;

        public CalculationResolver(IServiceProvider hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IPointsCalculationStrategy GetPointsCalculationStrategy(Game game)
        {
            // only '5-3-1-0' mode is currently supported
            var type = typeof(FiveThreeOneZeroPointsCalculationStrategy);

            var strategy = _hostingEnvironment.GetService(type) as IPointsCalculationStrategy;

            if (strategy == null)
                throw new Exception("Points calculation strategy not found!");

            return strategy;
        }

        public IProfitCalculationStrategy GetProfitCalculationStrategy(Game game)
        {
            // only 'The winner takes it all' mode is currently supported
            var type = typeof (TheWinneTakesItAllCalculationStrategy);

            var strategy = _hostingEnvironment.GetService(type) as IProfitCalculationStrategy;

            if (strategy == null)
                throw new Exception("Profit calculation strategy not found!");

            return strategy;
        }
    }
}