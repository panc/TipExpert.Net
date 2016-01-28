namespace TipExpert.Core.MatchSelection
{
    public interface IMatchSelectorFactory
    {
        IMatchSelector GetMatchSelector(MatchSelectionMode mode);
    }
}