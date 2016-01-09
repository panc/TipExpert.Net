using System;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public interface IMatchStore
    {
        Task Add(Match match);

        Task Remove(Match match);

        Task Update(Match match);

        Task<Match> GetById(Guid id);

        Task<Match[]> GetMatchesForLeague(Guid leagueId);
    }
}