using Battles.Api.Notifications.Dispatchers;
using Battles.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Api.Notifications
{
    public static class RegisterNotificationServices
    {
        public static IServiceCollection AddNotificationServices(this IServiceCollection @this)
        {
            @this.AddHostedService<NotificationProcessor>();
            @this.AddSingleton<OneSignalDispatcher>();
            @this.AddSingleton<EmailDispatcher>();
            @this.AddSingleton<DispatcherFactory>();
            @this.AddSingleton<INotificationSender, NotificationQueue>();
            @this.AddSingleton<INotificationQueue>(x => x.GetService<INotificationSender>());

            return @this;
        }
    }
}