using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using Battles.Rules.Matches.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
using Transmogrify;

namespace Battles.Application.Services.Matches.Commands
{
    public class PassRoundCommand : BaseRequest, IRequest<Response>
    {
        public int MatchId { get; set; }
    }

    public class PassRoundCommandHandler : IRequestHandler<PassRoundCommand, Response>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notifications;
        private readonly Library _library;

        public PassRoundCommandHandler(
            AppDbContext ctx,
            INotificationQueue notifications,
            Library library)
        {
            _ctx = ctx;
            _notifications = notifications;
            _library = library;
        }

        public async Task<Response> Handle(PassRoundCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .ThenInclude(x => x.User)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
                return Response.Fail(translationContext.Read("Match", "NotFound"));

            if (!match.CanPass(request.UserId))
                return Response.Fail(translationContext.Read("Match", "CantPass"));

            var user = match.GetUser(request.UserId);

            match.Videos.Add(new Video
            {
                VideoIndex = match.Videos.Count,
                UserIndex = user.Index,
                UserId = user.UserId,
                Empty = true,
            });

            var host = match.GetHost();
            var opponent = match.GetOpponent();

            host.SetGoFlagUpdatePassLock(host.CanGo, false, false);
            opponent.SetGoFlagUpdatePassLock(opponent.CanGo, false, false);

            match.Round++;

            match.Turn = match.GetTurnName();
            var finished = CopyCatHelper.CompleteCopyCat(match);
            match.LastUpdate = DateTime.Now;

            await _ctx.SaveChangesAsync(cancellationToken);

            if (finished)
            {
                var notificationMessage =
                    translationContext.Read("Notification", "PassedAndWon", user.User.DisplayName,
                                                     match.Round.ToString());

                _notifications.QueueNotification(notificationMessage,
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.MatchHistory,
                                                 match.GetOtherUserIds(user.UserId));
            }
            else
            {
                var notificationMessage =
                    translationContext.Read("Notification", "Passed", user.User.DisplayName,
                                                     match.Round.ToString());

                _notifications.QueueNotification(notificationMessage,
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.MatchActive,
                                                 match.GetOtherUserIds(user.UserId));
            }
            
            return Response.Ok(translationContext.Read("Match", "Passed"));
        }
    }
}