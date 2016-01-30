using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class MatchStore : IMatchStore
    {
        private readonly IMongoCollection<Match> _collection;

        public MatchStore(IMongoDatabase database)
        {
            _collection = database.GetCollection<Match>("matches");
        }

        public async Task Add(Match match)
        {
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

        public async Task<Match[]> GetMatchesForLeague(ObjectId leagueId)
        {
            return await _collection
                .Find(x => x.LeagueId == leagueId)
                .ToArrayAsync();
        }

        public async Task<Match> GetById(ObjectId id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}