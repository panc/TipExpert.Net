using System;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public interface IGameStore
    {
        Task Add(Game game);

        Task Remove(Game game);

        Task<Game> GetById(Guid id);

        Task<Game[]> GetGamesCreatedByUser(Guid userId);

        Task<Game[]> GetGamesUserIsInvitedTo(Guid userId);

        Task<Game[]> GetFinishedGames(Guid userId);

        Task<Game[]> GetGamesForMatch(Guid matchId);

        Task SaveChangesAsync();
    }
}