using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Extensions;
using Battles.Rules.Levels;
using Transmogrify;
using static System.String;

namespace Battles.Application.Services.Matches.Commands
{
    public class ForfeitMatchCommand : BaseRequest, IRequest<Response>
    {
        public int MatchId { get; set; }
    }

    public class ForfeitMatchCommandHandler : IRequestHandler<ForfeitMatchCommand, Response>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notification;
        private readonly Library _library;

        public ForfeitMatchCommandHandler(
            AppDbContext ctx,
            INotificationQueue notification,
            Library library)
        {
            _ctx = ctx;
            _notification = notification;
            _library = library;
        }

        //todo move to core level
        public async Task<Response> Handle(ForfeitMatchCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var match = await _ctx.Matches
                                  .Include(x => x.MatchUsers)
                                  .ThenInclude(x => x.User)
                                  .FirstOrDefaultAsync(x => x.Id == request.MatchId,
                                                       cancellationToken: cancellationToken);

            if (match == null)
            {
                return Response.Fail(translationContext.Read("Match", "NotFound"));
            }

            if (!match.CanGo(request.UserId))
            {
                return Response.Fail(translationContext.Read("Match", "CantForfeit"));
            }

            var host = match.GetHost();
            var opponent = match.GetOpponent();
            var notificationExtension = "";

            if (match.Round == 1 || (match.Mode == Mode.ThreeRoundPass && match.TurnType == TurnType.Blitz))
            {
                _ctx.Matches.Remove(match);
                notificationExtension = $". {translationContext.Read("Match", "Deleted")}";
            }
            else
            {
                if (match.IsTurn(MatchRole.Host))
                {
                    host.SetLoserAndLock(match.Round + 5)
                        .AwardExp(4 + match.Round);

                    opponent.SetWinnerAndLock(match.Round + 5)
                            .AwardExp(7 + match.Round);
                }
                else if (match.IsTurn(MatchRole.Opponent))
                {
                    host.SetWinnerAndLock(match.Round + 5)
                        .AwardExp(7 + match.Round);
                    opponent.SetLoserAndLock(match.Round + 5)
                            .AwardExp(4 + match.Round);
                }

                match.Status = Status.Complete;
                match.LastUpdate = DateTime.Now;
                match.Finished = DateTime.Now.GetFinishTime();
            }

            host.User.Hosting--;
            opponent.User.Joined--;

            await _ctx.SaveChangesAsync(cancellationToken);

            var notificationType = IsNullOrEmpty(notificationExtension)
                                       ? NotificationMessageType.MatchHistory
                                       : NotificationMessageType.Empty;

            var user = match.GetUser(request.UserId);
            var message =
                translationContext.Read("Notification", "Forfeited", user.User.DisplayName,
                                                 notificationExtension);

            _notification.QueueNotification(message,
                                            new[] {match.Id.ToString()}.DefaultJoin(),
                                            notificationType,
                                            match.GetOtherUserIds(request.UserId));

            return Response.Ok(translationContext.Read("Match", "Forfeited"));
        }
    }
}