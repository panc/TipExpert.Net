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
        private readonly IMatchStore _matchStore;

        public GameStore(IDataStoreConfiguration configuration, IUserStore userStore, IMatchStore matchStore)
            : base(configuration, FILE_NAME)
        {
            _userStore = userStore;
            _matchStore = matchStore;
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

        public Task<Game[]> GetGamesCreatedByUser(Guid userId)
        {
            return Task.Run(() => Entities
                .Where(x => x.CreatorId == userId && x.IsFinished == false)
                .ToArray());
        }

        public Task<Game[]> GetGamesUserIsInvitedTo(Guid userId)
        {
            return Task.Run(() => Entities
                .Where(x => !x.IsFinished && x.Players?.FirstOrDefault(p => p.UserId == userId) != null)
                .ToArray());
        }

        public Task<Game[]> GetFinishedGames(Guid userId)
        {
            return Task.Run(() => Entities
                .Where(x => x.IsFinished && x.Players?.FirstOrDefault(p => p.UserId == userId) != null)
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
            if (game.Players != null)
            {
                foreach (var player in game.Players)
                    player.User = await _userStore.GetById(player.UserId);
            }

            if (game.Matches != null)
            {
                foreach (var matchTips in game.Matches)
                    matchTips.Match = await _matchStore.GetById(matchTips.MatchId);
            }
        }
    }
}