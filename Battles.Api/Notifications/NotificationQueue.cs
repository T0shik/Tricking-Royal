using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Api.Notifications.Dispatchers;
using Battles.Enums;
using Battles.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrickingRoyal.Database;

namespace Battles.Api.Notifications
{
    public class NotificationQueue : INotificationSender
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DispatcherFactory _dispatcherFactory;
        private readonly ILogger<NotificationQueue> _logger;
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _notificationQueue;
        private readonly SemaphoreSlim _signal;

        public NotificationQueue(
            IServiceProvider serviceProvider,
            DispatcherFactory dispatcherFactory,
            ILogger<NotificationQueue> logger)
        {
            _serviceProvider = serviceProvider;
            _dispatcherFactory = dispatcherFactory;
            _logger = logger;
            _notificationQueue = new ConcurrentQueue<Func<CancellationToken, Task>>();
            _signal = new SemaphoreSlim(0);
        }

        public void QueueNotification(
            string message,
            string navigation,
            NotificationMessageType type,
            IEnumerable<string> targets,
            bool force = false,
            bool save = true)
        {
            _notificationQueue.Enqueue(ct => SaveAndSendNotifications(MessageFactory, targets, ct, force, save));
            _signal.Release();

            NotificationMessage MessageFactory(string target) =>
                new NotificationMessage
                {
                    UserInformationId = target,
                    Message = message,
                    Navigation = navigation,
                    Type = type,
                    Force = force,
                };
        }

        private async Task SaveAndSendNotifications(
            Func<string, NotificationMessage> messageFactory,
            IEnumerable<string> targets,
            CancellationToken ct,
            bool force,
            bool save)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    foreach (var target in targets)
                    {
                        _logger.LogInformation("Sending notifications to {0}", target);

                        var message = messageFactory(target);
                        if (save)
                        {
                            ctx.NotificationMessages.Add(message);
                            await ctx.SaveChangesAsync(ct);
                        }

                        var mediums = await ctx.NotificationConfigurations
                                               .AsNoTracking()
                                               .Where(x => x.UserInformationId == target && (x.Active || force))
                                               .ToListAsync(ct);

                        _logger.LogInformation("Found {0} mediums: {1}", mediums.Count, mediums);
                        foreach (var medium in mediums)
                        {
                            var dispatcher = _dispatcherFactory.GetDispatcher(medium.ConfigurationType);
                            _logger.LogInformation("Dispatching to {0}, destination: {1}",
                                                   medium.ConfigurationType.ToString(),
                                                   medium.NotificationId);
                            try
                            {
                                await dispatcher.SendNotification(message, medium.NotificationId, ct);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Failed to dispatch {0} notification.",
                                                 medium.ConfigurationType.ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Notification process failed.");
                }
            }
        }

        public async Task SendNotification(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _notificationQueue.TryDequeue(out var task);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            using (var source = new CancellationTokenSource())
            {
                source.CancelAfter(TimeSpan.FromMinutes(1));
                var timeoutToken = source.Token;

                await task(timeoutToken);
            }
        }
    }
}