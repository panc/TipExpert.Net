using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public interface IMatchFinalizationStrategy
    {
        Task UpdateGamesForMatch(Match match);
    }
}