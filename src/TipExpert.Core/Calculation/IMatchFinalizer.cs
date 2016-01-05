using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public interface IMatchFinalizer
    {
        Task UpdateGamesForMatch(Match match);
    }
}