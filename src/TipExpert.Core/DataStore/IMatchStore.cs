using System;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public interface IMatchStore
    {
        Task Add(Match match);

        Task Remove(Match match);

        Task<Match[]> GetAll();

        Task<Match> GetById(string id);

        Task SaveChangesAsync();

        Task<Match[]> GetMatchesForLeague(Guid leagueId);
    }
}