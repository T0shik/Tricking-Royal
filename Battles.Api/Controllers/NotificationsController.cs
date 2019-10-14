using Battles.Api.Infrastructure;
using Battles.Application.Services.Notifications.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications.Commands;
using Battles.Application.Services.Users.Commands;
using Battles.Application.Services.Users.Queries;
using Battles.Enums;

namespace Battles.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [Produces("application/json")]
    public class NotificationsController : BaseController
    {
        [HttpGet("")]
        public async Task<IActionResult> GetUsers(int index)
        {
            var notifications = await Mediator.Send(new GetNotificationsQuery
            {
                UserId = UserId,
                Index = index,
            });

            return Ok(notifications);
        }

        [HttpGet("config/{type}")]
        public async Task<IActionResult> GetNotificationConfiguration(int type)
        {
            var query = new GetUserNotificationConfigQuery
            {
                UserId = UserId,
                ConfigurationType = (NotificationConfigurationType) type,
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ConfigureNotificationsSubscription(
            [FromBody] ConfigureNotificationSubscriptionCommand command)
        {
            command.UserId = UserId;
            command.UserEmail = UserEmail;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPut("clear-all")]
        public async Task<IActionResult> ClearNotifications()
        {
            var result = await Mediator.Send(new ClearNotificationsCommand
            {
                UserId = UserId,
            });

            return Ok(result);
        }

        [HttpPut("{id}/touch")]
        public async Task<IActionResult> TouchNotification(int id)
        {
            await Mediator.Send(new TouchNotificationCommand
            {
                NotificationId = id,
                UserId = UserId,
            });

            return Ok();
        }
    }
}