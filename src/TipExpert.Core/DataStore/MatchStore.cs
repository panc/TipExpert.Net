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

        public async Task Update(Match match)
        {
            await _collection.ReplaceOneAsync(x => x.Id == match.Id, match);
        }

        public async Task<Match[]> GetMatchesForLeague(Guid leagueId)
        {
            return await _collection
                .Find(x => x.LeagueId == leagueId)
                .ToArrayAsync();
        }

        public async Task<Match> GetById(Guid id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}