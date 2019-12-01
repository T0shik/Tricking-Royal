using System.Threading.Tasks;
using Battles.Application.Services.Matches;
using Microsoft.AspNetCore.SignalR;

namespace Battles.Api.Workers.MatchUpdater
{
    public class MatchUpdaterNotifications : IMatchUpdaterNotifications
    {
        private readonly IHubContext<MatchUpdaterHub> _hub;

        public MatchUpdaterNotifications(IHubContext<MatchUpdaterHub> hub)
        {
            _hub = hub;
        }

        public Task NotifyMatchUpdated(string userId, int matchId)
        {
            var payload = new { matchId };
            return _hub.Clients.User(userId).SendAsync("MatchUpdateFinished", payload);
        }
    }
}