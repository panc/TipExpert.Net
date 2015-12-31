using System;
using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public class GameStore : StoreBase<Game>, IGameStore
    {
        private const string FILE_NAME = "games.json";

        public GameStore(string appDataPath)
            : base(appDataPath, FILE_NAME)
        {
        }

        public Task Add(Game game)
        {
            return Task.Run(() =>
            {
                game.Id = Guid.NewGuid();
                Entities.Add(game);
            });
        }

        public Task Remove(Game game)
        {
            return Task.Run(() => Entities.Remove(game));
        }

        public Task<Game[]> GetAll()
        {
            return Task.Run(() => Entities.ToArray());
        }

        public Task<Game[]> GetGamesCreatedByUser(Guid userId)
        {
            return Task.Run(() => Entities
                .Where(x => x.CreatorId == userId && x.IsFinished == false)
                .ToArray());
        }

        public Task<Game> GetById(Guid id)
        {
            return Task.Run(() =>
            {
                return Entities.FirstOrDefault(x => x.Id == id);
            });
        }
    }
}