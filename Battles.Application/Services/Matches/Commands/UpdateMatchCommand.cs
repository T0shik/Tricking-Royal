using System;
using TrickingRoyal.Database;
using Battles.Rules.Matches;
using Battles.Rules.Matches.Actions.Update;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
using Transmogrify;

namespace Battles.Application.Services.Matches.Commands
{
    public class UpdateMatchCommand : UpdateSettings, IRequest<Unit> { }

    public class UpdateMatchHandler : IRequestHandler<UpdateMatchCommand, Unit>
    {
        private readonly AppDbContext _ctx;
        private readonly MatchActionFactory _managerFactory;
        private readonly INotificationQueue _notifications;
        private readonly Library _library;

        public UpdateMatchHandler(
            AppDbContext ctx,
            MatchActionFactory managerFactory,
            INotificationQueue notifications,
            Library library)
        {
            _ctx = ctx;
            _managerFactory = managerFactory;
            _notifications = notifications;
            _library = library;
        }

        public async Task<Unit> Handle(UpdateMatchCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .ThenInclude(x => x.User)
                            .Include(x => x.Videos)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            _managerFactory
                .CreateMatchManager(match)
                .UpdateMatch(request);

            match.Updating = false;

            if (match.Mode == Mode.ThreeRoundPass
                && (match.TurnType == TurnType.Classic || match.TurnType == TurnType.Alternating)
                && match.Round > 3)
            {
                match.Status = Status.Pending;
                match.GetHost().SetLockUser();
                match.GetOpponent().SetLockUser();

                _ctx.Evaluations.Add(new Evaluation()
                {
                    MatchId = match.Id,
                    Match = match,
                    Expiry = DateTime.Now.AddDays(3),
                });

                await _ctx.SaveChangesAsync(cancellationToken);

                var notificationMessage = translationContext.Read("Notification", "MatchInTribunal");
                
                _notifications.QueueNotification(notificationMessage,
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.TribunalHistory,
                                                 match.GetUserIds());
            }
            else
            {
                await _ctx.SaveChangesAsync(cancellationToken);

                var currentUser = match.GetUser(request.UserId);
                
                var notificationMessage = translationContext.Read("Notification", "MatchUpdated", currentUser.User.DisplayName);
                
                _notifications.QueueNotification(notificationMessage,
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.MatchActive,
                                                 match.GetOtherUserIds(request.UserId));
            }

            return new Unit();
        }
    }
}