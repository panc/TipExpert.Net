using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class MatchStore : IMatchStore
    {
        private readonly IMongoCollection<Match> _collection;

        public MatchStore(MongoClient client)
        {
            var db = client.GetDatabase("TipExpert");
            _collection = db.GetCollection<Match>("matches");
        }

        public async Task Add(Match match)
        {
            match.Id = Guid.NewGuid();
            await _collection.InsertOneAsync(match);
        }

        public async Task Remove(Match match)
        {
            await _collection.DeleteOneAsync(x => x.Id == match.Id);
        }

        public async Task<Match[]> GetAll()
        {
            var result = await _collection.FindAsync(FilterDefinition<Match>.Empty);
            var list = await result.ToListAsync();
            return list.ToArray();
        }

        public async Task<Match[]> GetMatchesForLeague(Guid leagueId)
        {
            var result = await _collection.FindAsync(x => x.LeagueId == leagueId);
            var list = await result.ToListAsync();
            return list.ToArray();
        }

        public async Task<Match> GetById(Guid id)
        {
            var result = await _collection.FindAsync(x => x.Id == id);
            return await result.FirstOrDefaultAsync();
        }

        public Task SaveChangesAsync()
        {
            // Todo
            return Task.CompletedTask;
        }
    }
}