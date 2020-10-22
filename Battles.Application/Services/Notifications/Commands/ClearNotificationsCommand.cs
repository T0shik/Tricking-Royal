using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;
using Transmogrify;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Notifications.Commands
{
    public class ClearNotificationsCommand : BaseRequest, IRequest<Response>
    {   }

    public class ClearNotificationsCommandHandler : IRequestHandler<ClearNotificationsCommand, Response>
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

        public async Task<Response> Handle(ClearNotificationsCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var notifications = _ctx.NotificationMessages
                .Where(x => x.UserInformationId == request.UserId && x.New)
                .ToList();

            if (notifications.Count == 0)
                return Response.Ok(translationContext.Read("Notification", "Clear"));

            foreach (var n in notifications)
            {
                n.New = false;
            }

            await _ctx.SaveChangesAsync(cancellationToken);

            return Response.Ok(translationContext.Read("Notification", "Cleared"));
        }
    }
}