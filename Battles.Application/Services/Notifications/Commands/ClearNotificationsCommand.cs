using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Notifications.Commands
{
    public class ClearNotificationsCommand : IRequest<BaseResponse>
    {
        public string UserId { get; set; }
    }

    public class ClearNotificationsCommandHandler : IRequestHandler<ClearNotificationsCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;

        public ClearNotificationsCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(ClearNotificationsCommand request, CancellationToken cancellationToken)
        {
            var notifications = _ctx.NotificationMessages
                .Where(x => x.UserInformationId == request.UserId && x.New)
                .ToList();

            if (notifications.Count == 0)
            {
                return new BaseResponse("Notification are already cleared.", true);
            }

            foreach (var n in notifications)
            {
                n.New = false;
            }

            await _ctx.SaveChangesAsync(cancellationToken);

            return new BaseResponse("Notifications cleared.", true);
        }
    }
}