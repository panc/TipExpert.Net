using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core
{
    public interface IMatchStore
    {
        Task Add(Match match);

        Task Remove(Match match);

        Task Update(Match match);

        Task<Match> GetById(ObjectId id);

        Task<Match[]> GetMatchesForLeague(ObjectId leagueId);
    }
}