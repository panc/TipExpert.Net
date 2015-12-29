using System;
using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public class LeagueStore : StoreBase<League>, ILeagueStore
    {
        private const string FILE_NAME = "leagues.json";

        public LeagueStore(string appDataPath)
            : base(appDataPath, FILE_NAME)
        {
        }

        public Task Add(League league)
        {
            return Task.Run(() =>
            {
                league.Id = Guid.NewGuid();
                Entities.Add(league);
            });
        }

        public Task Remove(League league)
        {
            return Task.Run(() => Entities.Remove(league));
        }

        public Task<League[]> GetAll()
        {
            return Task.Run(() => Entities.ToArray());
        }

        public Task<League> GetById(Guid id)
        {
            return Task.Run(() =>
            {
                return Entities.FirstOrDefault(x => x.Id == id);
            });
        }
    }
}