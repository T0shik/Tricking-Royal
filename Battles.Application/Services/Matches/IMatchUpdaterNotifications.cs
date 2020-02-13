using System.Threading.Tasks;

namespace Battles.Application.Services.Matches
{
    public interface IMatchUpdaterNotifications
    {
        Task NotifyMatchUpdated(string userId, int matchId);
    }
}