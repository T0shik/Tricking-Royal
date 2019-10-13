using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Battles.Models;

namespace Battles.Api.Notifications.Dispatchers
{
    public interface IDispatcher
    {
        Task SendNotification(NotificationMessage message, string target, CancellationToken cancellationToken);
    }
}