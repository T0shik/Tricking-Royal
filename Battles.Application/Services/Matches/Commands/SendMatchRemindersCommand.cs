using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transmogrify;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Matches.Commands
{
    public class SendMatchRemindersCommand : IRequest<Unit> { }

    public class SendMatchRemindersCommandHandler : IRequestHandler<SendMatchRemindersCommand, Unit>
    {
        private readonly AppDbContext _ctx;
        private readonly ITranslator _translator;
        private readonly INotificationQueue _notificationQueue;

        public SendMatchRemindersCommandHandler(
            AppDbContext ctx,
            ITranslator translator,
            INotificationQueue notificationQueue)
        {
            _ctx = ctx;
            _translator = translator;
            _notificationQueue = notificationQueue;
        }

        public async Task<Unit> Handle(SendMatchRemindersCommand request, CancellationToken cancellationToken)
        {
            foreach (var d in new[] {1, 3})
            {
                var matches = GetMatchesToNotify(d);
                foreach (var match in matches)
                {
                    var turnUser = match.GetTurnUser();
                    var otherUserNames = match.GetOtherUsers(turnUser.UserId).Select(x => x.User.DisplayName);
                    var message = await _translator.GetTranslation("Notification", "MatchReminder",
                                                                   string.Join(", ", otherUserNames), d.ToString());
                    _notificationQueue.QueueNotification(message,
                                                         new[] {match.Id.ToString()}.DefaultJoin(),
                                                         NotificationMessageType.MatchActive,
                                                         new[] {turnUser.UserId},
                                                         force: true,
                                                         save: false);
                }
            }

            return new Unit();
        }

        private IEnumerable<Match> GetMatchesToNotify(int days) =>
            _ctx.Matches
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .Where(x => x.Status == Status.Active)
                .Where(x => EF.Functions.DateDiffDay(DateTime.Now, x.LastUpdate.AddDays(x.TurnDays)) == days)
                .ToList();
    }
}