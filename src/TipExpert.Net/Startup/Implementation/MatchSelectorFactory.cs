using System;
using TipExpert.Core;
using TipExpert.Core.MatchSelection;

namespace TipExpert.Net.Implementation
{
    public class MatchSelectorFactory : IMatchSelectorFactory
    {
        private readonly IServiceProvider _hostingEnvironment;

        public MatchSelectorFactory(IServiceProvider hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IMatchSelector GetMatchSelector(MatchSelectionMode mode)
        {
            var type = _MapMode(mode);
            return _hostingEnvironment.GetService(type) as IMatchSelector;
        }

        private Type _MapMode(MatchSelectionMode mode)
        {
            switch (mode)
            {
                case MatchSelectionMode.League:
                    return typeof(LeagueMatchSelector);
                case MatchSelectionMode.EM2016:
                    return typeof(Em2016MatchSelector);
                default:
                    throw new NotSupportedException($"Mode {mode} not supported!");
            }
        }
    }
}