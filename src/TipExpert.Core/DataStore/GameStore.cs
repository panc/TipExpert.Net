using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TipExpert.Core
{
    public class GameStore : IGameStore
    {
        private readonly IUserStore _userStore;
        private readonly IMatchStore _matchStore;
        private readonly IMongoCollection<Game> _collection;

        public GameStore(MongoClient client, IUserStore userStore, IMatchStore matchStore)
        {
            _userStore = userStore;
            _matchStore = matchStore;

            var db = client.GetDatabase("TipExpert");
            _collection = db.GetCollection<Game>("games");
        }

        public async Task Add(Game game)
        {
            game.Id = Guid.NewGuid();
            await _collection.InsertOneAsync(game);
        }

        public async Task Remove(Game game)
        {
            await _collection.DeleteOneAsync(x => x.Id == game.Id);
        }

        public async Task Update(Game game)
        {
            await _collection.ReplaceOneAsync(x => x.Id == game.Id, game);
        }

        public async Task<Game[]> GetGamesCreatedByUser(Guid userId)
        {
            return await _collection
                .Find(x => x.CreatorId == userId && x.IsFinished == false)
                .ToArrayAsync();
        }

        public async Task<Game[]> GetGamesUserIsInvitedTo(Guid userId)
        {
            return await _collection
                .Find(x => !x.IsFinished && x.Players.Any(p => p.UserId == userId))
                .ToArrayAsync();
        }

        public async Task<Game[]> GetFinishedGames(Guid userId)
        {
            return await _collection
                .Find(x => x.IsFinished && x.Players.Any(p => p.UserId == userId))
                .ToArrayAsync();
        }

        public async Task<Game[]> GetGamesForMatch(Guid matchId)
        {
            return await _collection
                .Find(x => x.Matches.Any(m => m.MatchId == matchId))
                .ToArrayAsync();
        }

        public async Task<Game> GetById(Guid id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}