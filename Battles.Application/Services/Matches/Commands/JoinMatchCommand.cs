using System;
using TrickingRoyal.Database;
using Battles.Rules.Matches.Actions.Join;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Extensions;
using Transmogrify;

namespace Battles.Application.Services.Matches.Commands
{
    public class JoinMatchCommand : BaseRequest, IRequest<Response>
    {
        public int MatchId { get; set; }
    }

    public class JoinMatchCommandHandler : IRequestHandler<JoinMatchCommand, Response>
    {
        private readonly AppDbContext _ctx;
        private readonly MatchDoorman _matchDoorman;
        private readonly INotificationQueue _notification;
        private readonly Library _library;

        public JoinMatchCommandHandler(
            AppDbContext ctx,
            MatchDoorman matchDoorman,
            INotificationQueue notification,
            Library library)
        {
            _ctx = ctx;
            _matchDoorman = matchDoorman;
            _notification = notification;
            _library = library;
        }

        public async Task<Response> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .ThenInclude(x => x.User)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
                return Response.Fail(translationContext.Read("Match", "NotFound"));

            var currentUser = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (currentUser == null)
                return Response.Fail(translationContext.Read("User", "NotFound"));

            try
            {
                _matchDoorman.AddOpponent(match, currentUser);
            }
            catch (Exception e)
            {
                return Response.Fail(e.Message);
            }

            await _ctx.SaveChangesAsync(cancellationToken);

            var notificationMessage =
                translationContext.Read("Notification", "JoinedMatch", currentUser.DisplayName);

            _notification.QueueNotification(notificationMessage,
                                            new[] {match.Id.ToString()}.DefaultJoin(),
                                            NotificationMessageType.MatchActive,
                                            new[] {match.GetHost().UserId});

            return Response.Ok(translationContext.Read("Match", "Joined"));
        }
    }
}