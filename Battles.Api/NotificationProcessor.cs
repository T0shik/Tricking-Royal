using System.Threading;
using System.Threading.Tasks;
using Battles.Api.Notifications;
using Microsoft.Extensions.Hosting;

namespace Battles.Api
{
    public class NotificationProcessor : BackgroundService
    {
        private readonly INotificationSender _notificationSender;

        public NotificationProcessor(INotificationSender notificationSender)
        {
            _notificationSender = notificationSender;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _notificationSender.SendNotification(stoppingToken);
            }
        }
    }
}