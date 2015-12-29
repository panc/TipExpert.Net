using System;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public interface ILeagueStore
    {
        Task Add(League league);

        Task Remove(League league);

        Task<League[]> GetAll();

        Task<League> GetById(Guid id);

        Task SaveChangesAsync();
    }
}