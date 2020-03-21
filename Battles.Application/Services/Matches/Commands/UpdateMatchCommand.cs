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
        private readonly AppDbContext _dbContext;
        private readonly MatchActionFactory _managerFactory;
        private readonly INotificationQueue _notifications;
        private readonly Library _library;

        public UpdateMatchHandler(
            AppDbContext dbContext,
            MatchActionFactory managerFactory,
            INotificationQueue notifications,
            Library library)
        {
            _dbContext = dbContext;
            _managerFactory = managerFactory;
            _notifications = notifications;
            _library = library;
        }

        public async Task<Unit> Handle(UpdateMatchCommand command, CancellationToken ct)
        {
            var match = _dbContext.Matches
                                  .Include(x => x.MatchUsers)
                                  .ThenInclude(x => x.User)
                                  .Include(x => x.Videos)
                                  .FirstOrDefault(x => x.Id == command.MatchId);

            _managerFactory
                .CreateMatchManager(match)
                .UpdateMatch(command);

            match.Updating = false;

            if (match.Mode == Mode.ThreeRoundPass
                && (match.TurnType == TurnType.Classic || match.TurnType == TurnType.Alternating)
                && match.Round > 3)
            {
                match.Status = Status.Pending;
                match.GetHost().SetLockUser();
                match.GetOpponent().SetLockUser();

                _dbContext.Evaluations.Add(new Evaluation()
                {
                    MatchId = match.Id,
                    Match = match,
                    Expiry = DateTime.Now.AddDays(3),
                });

                await _dbContext.SaveChangesAsync(ct);

                foreach (var userId in match.GetUserIds())
                {
                    var userLanguage = await _dbContext.UserLanguage(userId, ct);
                    var translationContext = _library.GetContext(userLanguage);
                    var notificationMessage = translationContext.Read("Notification", "MatchInTribunal");

                    _notifications.QueueNotification(notificationMessage,
                                                     new[] {match.Id.ToString()}.DefaultJoin(),
                                                     NotificationMessageType.TribunalHistory,
                                                     new[] {userId});
                }
            }
            else
            {
                await _dbContext.SaveChangesAsync(ct);

                var currentUser = match.GetUser(command.UserId);

                foreach (var userId in match.GetOtherUserIds(command.UserId))
                {
                    var userLanguage = await _dbContext.UserLanguage(userId, ct);
                    var translationContext = _library.GetContext(userLanguage);
                    var notificationMessage =
                        translationContext.Read("Notification", "MatchUpdated", currentUser.User.DisplayName);
                    _notifications.QueueNotification(notificationMessage, new[] {match.Id.ToString()}.DefaultJoin(),
                                                     NotificationMessageType.MatchActive, new[] {userId});
                }
            }

            return new Unit();
        }
    }
}