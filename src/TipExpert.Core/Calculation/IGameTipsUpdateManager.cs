using System.Threading.Tasks;

namespace TipExpert.Core.Strategy
{
    public interface IGameTipsUpdateManager
    {
        Task UpdateGamesForMatch(Match match);
    }
}