using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Interfaces;

namespace Battles.Api.Workers.Notifications
{
    public interface INotificationSender : INotificationQueue
    {
        Task SendNotification(CancellationToken cancellationToken);
    }
}