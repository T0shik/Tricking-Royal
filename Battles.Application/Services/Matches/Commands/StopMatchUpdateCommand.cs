using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Enums;
using Battles.Extensions;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transmogrify;
using TrickingRoyal.Database;
using TrickingRoyal.Database.Queries;

namespace Battles.Application.Services.Matches.Commands
{
    public class StopMatchUpdateCommand : BaseRequest, IRequest<Unit>
    {
        public int MatchId { get; set; }
    }

    public class StopMatchUpdateCommandHandler : IRequestHandler<StopMatchUpdateCommand, Unit>
    {
        private readonly AppDbContext _dbContext;
        private readonly INotificationQueue _notifications;
        private readonly Library _library;

        public StopMatchUpdateCommandHandler(
            AppDbContext dbContext,
            INotificationQueue notifications,
            Library library)
        {
            _dbContext = dbContext;
            _notifications = notifications;
            _library = library;
        }

        public async Task<Unit> Handle(StopMatchUpdateCommand command, CancellationToken ct)
        {
            var userLanguage = await _dbContext.UserLanguage(command.UserId, ct);
            var translationContext = _library.GetContext(userLanguage);
            var match = await _dbContext.Matches.FirstOrDefaultAsync(x => x.Id == command.MatchId,
                                                               cancellationToken: ct);

            match.Updating = false;

            await _dbContext.SaveChangesAsync(ct);

            var notificationMessage = translationContext.Read("Notification", "StopMatchUpdate");

            _notifications.QueueNotification(notificationMessage,
                                             new[] {match.Id.ToString()}.DefaultJoin(),
                                             NotificationMessageType.MatchActive,
                                             new[] {command.UserId});

            return new Unit();
        }
    }
}