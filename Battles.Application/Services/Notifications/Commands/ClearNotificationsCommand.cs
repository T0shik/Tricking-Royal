using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;
using Transmogrify;
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
        private readonly Library _library;

        public ClearNotificationsCommandHandler(
            AppDbContext ctx, 
            Library library)
        {
            _ctx = ctx;
            _library = library;
        }

        public async Task<BaseResponse> Handle(ClearNotificationsCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var notifications = _ctx.NotificationMessages
                .Where(x => x.UserInformationId == request.UserId && x.New)
                .ToList();

            if (notifications.Count == 0)
                return BaseResponse.Ok(translationContext.Read("Notification", "Clear"));

            foreach (var n in notifications)
            {
                n.New = false;
            }

            await _ctx.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(translationContext.Read("Notification", "Cleared"));
        }
    }
}