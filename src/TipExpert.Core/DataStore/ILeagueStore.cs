using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core
{
    public interface ILeagueStore
    {
        Task Add(League league);

        Task Remove(League league);

        Task Update(League league);

        Task<League[]> GetAll();

        Task<League> GetById(ObjectId id);
    }
}