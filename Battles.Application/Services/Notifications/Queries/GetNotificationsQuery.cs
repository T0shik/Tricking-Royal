using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Battles.Application.Services.Notifications.Queries
{
    public class GetNotificationsQuery : IRequest<IEnumerable<NotificationsViewModel>>
    {
        public string UserId { get; set; }
        public int Index { get; set; }
    }

    public class GetNotificationsQueryHandler
        : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationsViewModel>>
    {
        private readonly AppDbContext _ctx;

        public GetNotificationsQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<IEnumerable<NotificationsViewModel>> Handle(GetNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            return await _ctx.NotificationMessages
                .Where(x => x.UserInformationId == request.UserId)
                .OrderByDescending(x => x.TimeStamp)
                .GrabSegment(request.Index, 7)
                .Select(NotificationsViewModel.Projection)
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}