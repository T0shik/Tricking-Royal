using Battles.Api.Workers.Notifications.Dispatchers;
using Battles.Application.Services.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Api.Workers.Notifications
{
    public static class NotificationServices
    {
        public static IServiceCollection AddNotificationWorker(this IServiceCollection @this)
        {
            @this.AddSingleton<OneSignalDispatcher>();
            @this.AddScoped<EmailDispatcher>();
            @this.AddSingleton<DispatcherFactory>();
            @this.AddSingleton<INotificationQueue, NotificationQueue>();
            @this.AddSingleton<IWorker, NotificationSender>();
            return @this;
        }
    }
}