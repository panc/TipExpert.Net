using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class LeagueStore : ILeagueStore
    {
        private readonly IMongoCollection<League> _collection;

        public LeagueStore(MongoClient client)
        {
            var db = client.GetDatabase("TipExpert");
            _collection = db.GetCollection<League>("leagues");
        }

        public async Task Add(League league)
        {
            league.Id = Guid.NewGuid();
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
            var list = await _collection.Find(FilterDefinition<League>.Empty).ToListAsync();
            return list.ToArray();
        }

        public async Task<League> GetById(Guid id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}