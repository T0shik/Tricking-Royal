using TrickingRoyal.Database;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Enums;
using Battles.Models;

namespace Battles.Application.Services.Users.Commands
{
    public class ConfigureNotificationSubscriptionCommand : BaseRequest, IRequest<int>
    {
        public string NotificationId { get; set; }
        public int Type { get; set; }
        public bool Active { get; set; }
    }

    public class ConfigureNotificationSubscriptionCommandHandler
        : IRequestHandler<ConfigureNotificationSubscriptionCommand, int>
    {
        private readonly AppDbContext _ctx;

        public ConfigureNotificationSubscriptionCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<int> Handle(ConfigureNotificationSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            return request.Type == (int) NotificationConfigurationType.Email
                ? ConfigureEmailNotifications(request, cancellationToken)
                : ConfigureNotification(request, cancellationToken);
        }

        private Task<int> ConfigureEmailNotifications(
            ConfigureNotificationSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            var notification = _ctx.NotificationConfigurations
                .FirstOrDefault(x => x.UserInformationId == request.UserId
                                     && x.ConfigurationType == NotificationConfigurationType.Email);

            var email = string.IsNullOrEmpty(request.NotificationId)
                ? request.UserEmail
                : request.NotificationId;

            if (notification == null)
            {
                _ctx.NotificationConfigurations.Add(new NotificationConfiguration
                {
                    UserInformationId = request.UserId,
                    NotificationId = email,
                    ConfigurationType = NotificationConfigurationType.Email,
                    Active = true,
                });
            }
            else
            {
                notification.NotificationId = email;
                notification.Active = request.Active;
            }

            return _ctx.SaveChangesAsync(cancellationToken);
        }

        private Task<int> ConfigureNotification(
            ConfigureNotificationSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            var notification = _ctx.NotificationConfigurations
                .FirstOrDefault(x => x.UserInformationId == request.UserId
                                     && x.NotificationId == request.NotificationId);

            if (notification == null)
            {
                _ctx.NotificationConfigurations.Add(new NotificationConfiguration
                {
                    UserInformationId = request.UserId,
                    NotificationId = request.NotificationId,
                    ConfigurationType = (NotificationConfigurationType) request.Type,
                    Active = true,
                });
            }
            else
            {
                notification.Active = request.Active;
            }

            return _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}