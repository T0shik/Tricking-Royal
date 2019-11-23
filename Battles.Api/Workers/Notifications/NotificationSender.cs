using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;

namespace Battles.Api.Workers.Notifications
{
    public class NotificationSender : IWorker
    {
        private readonly INotificationQueue _notificationQueue;

        public NotificationSender(INotificationQueue notificationQueue)
        {
            _notificationQueue = notificationQueue;
        }

        public Task StartAsync(CancellationToken cancellationToken) => _notificationQueue.Pop(cancellationToken);
    }
}