using System;
using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public class MatchStore : StoreBase<Match>, IMatchStore
    {
        private const string FILE_NAME = "matches.json";

        public MatchStore(string appDataPath)
            : base(appDataPath, FILE_NAME)
        {
        }

        public Task Add(Match match)
        {
            return Task.Run(() =>
            {
                match.Id = Guid.NewGuid();
                Entities.Add(match);
            });
        }

        public Task Remove(Match match)
        {
            return Task.Run(() => Entities.Remove(match));
        }

        public Task<Match[]> GetAll()
        {
            return Task.Run(() => Entities.ToArray());
        }

        public Task<Match> GetById(string id)
        {
            return Task.Run(() =>
            {
                return Entities.FirstOrDefault(x => x.Id.ToString() == id);
            });
        }

        public Task<Match[]> GetMatchesForLeague(Guid leagueId)
        {
            return Task.Run(() =>
            {
                return Entities.Where(x => x.LeagueId == leagueId).ToArray();
            });
        }
    }
}