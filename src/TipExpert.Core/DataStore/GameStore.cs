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

        public async Task<Game[]> GetGamesCreatedByUser(Guid userId)
        {
            var result = await _collection.FindAsync(x => x.CreatorId == userId && x.IsFinished == false);
            var list = await result.ToListAsync();
            return list.ToArray();
        }

        public async Task<Game[]> GetGamesUserIsInvitedTo(Guid userId)
        {
            var result = await _collection.FindAsync(x => !x.IsFinished && x.Players.FirstOrDefault(p => p.UserId == userId) != null);
            var list = await result.ToListAsync();
            return list.ToArray();
        }

        public async Task<Game[]> GetFinishedGames(Guid userId)
        {
            var result = await _collection.FindAsync(x => x.IsFinished && x.Players.FirstOrDefault(p => p.UserId == userId) != null);
            var list = await result.ToListAsync();
            return list.ToArray();
        }

        public async Task<Game[]> GetGamesForMatch(Guid matchId)
        {
            var result = await _collection.FindAsync(x => x.Matches.FirstOrDefault(m => m.MatchId == matchId) != null);
            var list = await result.ToListAsync();
            return list.ToArray();
        }

        public Task SaveChangesAsync()
        {
            // todo
            return Task.CompletedTask;
        }

        public async Task<Game> GetById(Guid id)
        {
            var result = await _collection.FindAsync(x => x.Id == id);
            return await result.FirstOrDefaultAsync();
        }
    }
}