using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Notifications.Commands
{
    public class TouchNotificationCommand : IRequest<BaseResponse>
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; }
    }

    public class TouchNotificationCommandHandler : IRequestHandler<TouchNotificationCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;

        public TouchNotificationCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(TouchNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = _ctx.NotificationMessages
                .FirstOrDefault(x => x.Id == request.NotificationId
                                     && x.UserInformationId == request.UserId);

            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            notification.New = false;

            await _ctx.SaveChangesAsync(cancellationToken);

            return new BaseResponse("Notification touched", true);
        }
    }
}