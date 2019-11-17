using System.Threading;
using System.Threading.Tasks;
using Battles.Api.Workers.Notifications;
using Microsoft.Extensions.Hosting;

namespace Battles.Api.Workers
{
    public class BackgroundServices : BackgroundService
    {
        private readonly INotificationSender _notificationSender;

        public BackgroundServices(INotificationSender notificationSender)
        {
            _notificationSender = notificationSender;
        }
        
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(new Task[] {
                NotificationService(cancellationToken),
                MatchUpdater(cancellationToken),
            });
        }

        public async Task NotificationService(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _notificationSender.SendNotification(cancellationToken);
            }
        }
        
        public async Task MatchUpdater(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                
            }
        }
    }
}