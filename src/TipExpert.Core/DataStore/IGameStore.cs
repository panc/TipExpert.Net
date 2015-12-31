using System;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public interface IGameStore
    {
        Task Add(Game game);

        Task Remove(Game game);

        Task<Game> GetById(Guid id);

        Task<Game[]> GetAll();

        Task<Game[]> GetGamesCreatedByUser(Guid userId);

        Task SaveChangesAsync();
    }
}