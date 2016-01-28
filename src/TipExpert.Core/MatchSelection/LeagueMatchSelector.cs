using System.Collections.Generic;
using System.Threading.Tasks;

namespace TipExpert.Core.MatchSelection
{
    public class LeagueMatchSelector : IMatchSelector
    {
        public Task<List<Match>> GetMatches(string matchesMetadata)
        {
            throw new System.NotImplementedException();
        }
    }
}