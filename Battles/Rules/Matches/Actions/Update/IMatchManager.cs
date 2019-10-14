namespace Battles.Rules.Matches.Actions.Update
{
    public interface IMatchManager
    {
        void UpdateMatch(UpdateSettings command);
    }
}