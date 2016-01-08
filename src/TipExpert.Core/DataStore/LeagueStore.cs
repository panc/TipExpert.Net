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

        public async Task<League[]> GetAll()
        {
            var result = await _collection.FindAsync(FilterDefinition<League>.Empty);
            var list = await result.ToListAsync();
            return list.ToArray();
        }

        public async Task<League> GetById(Guid id)
        {
            var result = await _collection.FindAsync(x => x.Id == id);
            return await result.FirstOrDefaultAsync();
        }

        public Task SaveChangesAsync()
        {
            // TODO
            return Task.CompletedTask;
        }
    }
}