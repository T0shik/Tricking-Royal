using System;
using Battles.Enums;
using Battles.Models;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Api.Notifications.Dispatchers
{
    public class DispatcherFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DispatcherFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDispatcher GetDispatcher(NotificationConfigurationType type)
        {
            switch (type)
            {
                case NotificationConfigurationType.App:
                    return _serviceProvider.GetService<OneSignalDispatcher>();
                case NotificationConfigurationType.Web:
                    return _serviceProvider.GetService<OneSignalDispatcher>();
                case NotificationConfigurationType.Email:
                    return _serviceProvider.GetService<EmailDispatcher>();
                case NotificationConfigurationType.Mobile:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type,
                        "Invalid NotificationConfigurationType selected.");
            }
        }
    }
}