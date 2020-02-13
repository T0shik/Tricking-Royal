using Battles.Api.Workers.MatchUpdater;
using Battles.Api.Workers.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Api.Workers
{
    public static class RegisterWorkers
    {
        public static IServiceCollection AddWorkers(this IServiceCollection @this)
        {
            return @this.AddHostedService<WorkerManager>()
                        .AddNotificationWorker()
                        .AddMatchUpdaterWorker();
        }
    }
}