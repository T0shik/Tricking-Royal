﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Notifications.Commands
{
    public class TouchNotificationCommand : BaseRequest, IRequest<Unit>
    {
        public int NotificationId { get; set; }
    }

    public class TouchNotificationCommandHandler : IRequestHandler<TouchNotificationCommand, Unit>
    {
        private readonly AppDbContext _ctx;

        public TouchNotificationCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Unit> Handle(TouchNotificationCommand request, CancellationToken cancellationToken)
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

            return new Unit();
        }
    }
}