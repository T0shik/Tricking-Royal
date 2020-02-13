using TrickingRoyal.Database;
using Battles.Application.Services.Evaluations.Commands;
using Battles.Application.Services.Matches.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
using Battles.Rules.Matches.Extensions;
using Microsoft.Extensions.Logging;

namespace Battles.Application.Jobs
{
    public class HangfireJobs : IHangfireJobs
    {
        private readonly AppDbContext _ctx;
        private readonly IMediator _mediator;
        private readonly INotificationQueue _notificationQueue;
        private readonly ILogger<HangfireJobs> _logger;

        public HangfireJobs(
            AppDbContext ctx, 
            IMediator mediator, 
            INotificationQueue notificationQueue,
            ILogger<HangfireJobs> logger)
        {
            _ctx = ctx;
            _mediator = mediator;
            _notificationQueue = notificationQueue;
            _logger = logger;
        }

        public async Task CloseExpiredEvaluations()
        {
            foreach (var evaluation in GetExpiredEvaluations())
                if (evaluation.EvaluationType == EvaluationT.Complete)
                    await _mediator.Send(new CloseCompleteEvaluationCommand(evaluation));
                else
                    await _mediator.Send(new CloseFlagEvaluationCommand(evaluation));
        }

        public void MatchReminder()
        {
            var days = new[] {1, 3};
            foreach (var d in days)
            {
                var matches = GetMatchesToNotify(d);
                foreach (var match in matches)
                {
                    var turnUser = match.GetTurnUser();
                    var otherUserNames = match.GetOtherUsers(turnUser.UserId).Select(x => x.User.DisplayName);
                    var message =
                        $"Your match with {string.Join(", ", otherUserNames)} is going to expire in {d} days. You will lose reputation if you don't respond.";
                    _notificationQueue.QueueNotification(message,
                                                         new[] {match.Id.ToString()}.DefaultJoin(),
                                                         NotificationMessageType.MatchActive,
                                                         new[] {turnUser.UserId},
                                                         force: true,
                                                         save: false);
                }
            }
        }

        public async Task CloseExpiredMatches()
        {
            foreach (var match in GetExpiredMatches())
                await _mediator.Send(new TimeoutMatchCommand(match));
        }

        private IEnumerable<Match> GetMatchesToNotify(int days) =>
            _ctx.Matches
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .Where(x => x.Status == Status.Active)
                .Where(x => EF.Functions.DateDiffDay(DateTime.Now, x.LastUpdate.AddDays(x.TurnDays)) == days)
                .ToList();

        private IEnumerable<Match> GetExpiredMatches() =>
            _ctx.Matches
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .Where(x => x.Status == Status.Active)
                .Where(x => EF.Functions.DateDiffHour(x.LastUpdate, DateTime.Now) >= x.TurnDays * 24)
                .ToList();

        private IEnumerable<Evaluation> GetExpiredEvaluations() =>
            _ctx.Evaluations
                .Include(x => x.Match)
                .ThenInclude(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .Include(x => x.Decisions)
                .ThenInclude(x => x.User)
                .Where(x => !x.Complete)
                .Where(x => EF.Functions.DateDiffHour(DateTime.Now, x.Expiry) < 0)
                .ToList();
    }
}