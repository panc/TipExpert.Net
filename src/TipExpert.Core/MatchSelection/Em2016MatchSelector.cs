using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core.MatchSelection
{
    public class Em2016MatchSelector : IMatchSelector
    {
        private readonly ILeagueStore _leagueStore;
        private readonly IMatchStore _matchStore;

        public Em2016MatchSelector(ILeagueStore leagueStore, IMatchStore matchStore)
        {
            _leagueStore = leagueStore;
            _matchStore = matchStore;
        }

        public async Task<List<Match>> GetMatches(string matchesMetadata)
        {
            var leagues = await _leagueStore.GetAll();
            var matches = await _matchStore.GetMatchesForLeague(leagues[0].Id);

            return matches.ToList();
        }
    }
}