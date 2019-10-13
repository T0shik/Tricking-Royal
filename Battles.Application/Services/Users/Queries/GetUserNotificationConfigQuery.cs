using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Users.Queries
{
    public class GetUserNotificationConfigQuery : IRequest<NotificationConfigViewModel>
    {
        public string UserId { get; set; }
        public NotificationConfigurationType ConfigurationType { get; set; }
    }

    public class GetUserNotificationConfigQueryHandler
        : IRequestHandler<GetUserNotificationConfigQuery, NotificationConfigViewModel>
    {
        private readonly AppDbContext _ctx;

        public GetUserNotificationConfigQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public Task<NotificationConfigViewModel> Handle(GetUserNotificationConfigQuery request,
            CancellationToken cancellationToken)
        {
            return _ctx.NotificationConfigurations
                .Where(x => x.UserInformationId == request.UserId
                            && x.ConfigurationType == request.ConfigurationType)
                .Select(NotificationConfigViewModel.Projection)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}