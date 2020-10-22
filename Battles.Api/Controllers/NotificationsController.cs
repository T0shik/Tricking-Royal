using System.Collections.Generic;
using Battles.Api.Infrastructure;
using Battles.Application.Services.Notifications.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications.Commands;
using Battles.Application.Services.Users.Commands;
using Battles.Application.Services.Users.Queries;
using Battles.Application.ViewModels;
using Battles.Enums;
using MediatR;

namespace Battles.Api.Controllers
{
    [Authorize]
    public class NotificationsController : BaseController
    {
        public NotificationsController(IMediator mediator)
            : base(mediator) { }

        [HttpGet]
        public Task<IEnumerable<NotificationsViewModel>> GetUsers([FromQuery] GetNotificationsQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpGet("config/{type}")]
        public Task<NotificationConfigViewModel> GetNotificationConfiguration(
            [FromRoute] GetUserNotificationConfigQuery query)
        {
            // var query = new GetUserNotificationConfigQuery
            // {
            //     UserId = UserId,
            //     ConfigurationType = (NotificationConfigurationType) type,
            // };
            return Mediator.Send(query);
        }

        [HttpPost]
        public Task<int> ConfigureNotificationsSubscription([FromBody] ConfigureNotificationSubscriptionCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPut("clear-all")]
        public Task<Response> ClearNotifications()
        {
            return Mediator.Send(new ClearNotificationsCommand());
        }

        [HttpPut("{notificationId}/touch")]
        public Task<Unit> TouchNotification([FromRoute] TouchNotificationCommand command)
        {
            return Mediator.Send(command);
        }
    }
}