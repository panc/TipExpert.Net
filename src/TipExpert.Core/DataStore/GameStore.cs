﻿using System;
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

        public GameStore(IMongoDatabase database, IUserStore userStore, IMatchStore matchStore)
        {
            _userStore = userStore;
            _matchStore = matchStore;
            _collection = database.GetCollection<Game>("games");
        }

        public async Task Add(Game game)
        {
            game.Id = Guid.NewGuid();
            await _collection.InsertOneAsync(game);
            await _PopulateRelations(game);
        }

        public async Task Remove(Game game)
        {
            await _collection.DeleteOneAsync(x => x.Id == game.Id);
        }

        public async Task Update(Game game)
        {
            await _collection.ReplaceOneAsync(x => x.Id == game.Id, game);
            await _PopulateRelations(game);
        }

        public async Task<Game[]> GetGamesCreatedByUser(Guid userId)
        {
            var games = await _collection
                .Find(x => x.CreatorId == userId && x.IsFinished == false)
                .ToArrayAsync();

            await _PopulateRelations(games);

            return games;
        }

        public async Task<Game[]> GetGamesUserIsInvitedTo(Guid userId)
        {
            var games = await _collection
                .Find(x => !x.IsFinished && x.Players.Any(p => p.UserId == userId))
                .ToArrayAsync();

            await _PopulateRelations(games);

            return games;
        }

        public async Task<Game[]> GetFinishedGames(Guid userId)
        {
            var games = await _collection
                .Find(x => x.IsFinished && x.Players.Any(p => p.UserId == userId))
                .ToArrayAsync();

            await _PopulateRelations(games);

            return games;
        }

        public async Task<Game[]> GetGamesForMatch(Guid matchId)
        {
            var games = await _collection
                .Find(x => x.Matches.Any(m => m.MatchId == matchId))
                .ToArrayAsync();

            await _PopulateRelations(games);

            return games;
        }

        public async Task<Game> GetById(Guid id)
        {
            var game = await _collection
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();

            await _PopulateRelations(game);

            return game;
        }

        private async Task _PopulateRelations(Game[] games)
        {
            foreach (var game in games)
                await _PopulateRelations(game);
        }

        private async Task _PopulateRelations(Game game)
        {
            if (game.Players != null)
            {
                foreach (var player in game.Players)
                    player.User = await _userStore.GetById(player.UserId);
            }

            if (game.Matches != null)
            {
                foreach (var mt in game.Matches)
                {
                    mt.Match = await _matchStore.GetById(mt.MatchId);
                }
            }
        }
    }
}