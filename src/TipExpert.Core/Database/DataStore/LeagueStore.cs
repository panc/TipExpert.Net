using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class LeagueStore : ILeagueStore
    {
        private readonly IMongoCollection<League> _collection;

        public LeagueStore(IMongoDatabase database)
        {
            _collection = database.GetCollection<League>("leagues");
        }

        public async Task Add(League league)
        {
            await _collection.InsertOneAsync(league);
        }

        public async Task Remove(League league)
        {
            await _collection.DeleteOneAsync(x => x.Id == league.Id);
        }

        public async Task Update(League league)
        {
            await _collection.ReplaceOneAsync(x => x.Id == league.Id, league);
        }

        public async Task<League[]> GetAll()
        {
            return await _collection
                .Find(FilterDefinition<League>.Empty)
                .ToArrayAsync();
        }

        public async Task<League> GetById(ObjectId id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}