using System.Threading.Tasks;

namespace TipExpert.Core.Calculation
{
    public interface IGameTipsUpdateManager
    {
        Task UpdateGamesForMatch(Match match);
    }
}