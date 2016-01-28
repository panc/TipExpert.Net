using System.Collections.Generic;
using System.Threading.Tasks;

namespace TipExpert.Core.MatchSelection
{
    public interface IMatchSelector
    {
        Task<List<Match>> GetMatches(string matchesMetadata);
    }
}