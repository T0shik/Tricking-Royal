namespace Battles.Interfaces
{
    public interface IOpponentChecker
    {
        bool AreOpponents(string host, string opponent);
        void LoadOpponents(string userId);
    }
}