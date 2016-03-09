using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core
{
    public interface IGameStore
    {
        Task Add(Game game);

        Task Remove(Game game);

        Task Update(Game game);

        Task<Game> GetById(ObjectId id);

        Task<Game[]> GetGamesForUser(ObjectId userId);

        Task<Game[]> GetFinishedGames(ObjectId userId);

        Task<Game[]> GetGamesForMatch(ObjectId matchId);
    }
}