using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public class GameStore : StoreBase<Game>, IGameStore
    {
        private const string FILE_NAME = "games.json";

        private readonly IUserStore _userStore;

        public GameStore(IDataStoreConfiguration configuration, IUserStore userStore)
            : base(configuration, FILE_NAME)
        {
            _userStore = userStore;
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

        protected override async void OnEntitiesLoaded(List<Game> entities)
        {
            foreach (var game in entities)
                await _LoadRelations(game);
        }

        protected override async void OnEntitiesSaved(List<Game> entities)
        {
            foreach (var game in entities)
                await _LoadRelations(game);
        }

        private async Task _LoadRelations(Game game)
        {
            foreach (var player in game.Players)
            {
                player.User = await _userStore.GetById(player.UserId);
            }
        }
    }
}