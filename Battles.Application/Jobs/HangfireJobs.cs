using TrickingRoyal.Database;
using Battles.Application.Services.Evaluations.Commands;
using Battles.Application.Services.Matches.Commands;
using Battles.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Battles.Enums;
using Battles.Models;

namespace Battles.Application.Jobs
{
    public class HangfireJobs : IHangfireJobs
    {
        private readonly AppDbContext _ctx;
        private readonly IMediator _mediator;

        public HangfireJobs(AppDbContext ctx, IMediator mediator)
        {
            _ctx = ctx;
            _mediator = mediator;
        }

        public async Task CloseExpiredEvaluations()
        {
            foreach (var evaluation in GetExpiredEvaluations())
                if (evaluation.EvaluationType == EvaluationT.Complete)
                    await _mediator.Send(new CloseCompleteEvaluationCommand(evaluation));
                else
                    await _mediator.Send(new CloseFlagEvaluationCommand(evaluation));
        }

        public async Task CloseExpiredMatches()
        {
            foreach (var match in GetExpiredMatches())
                await _mediator.Send(new TimeoutMatchCommand(match));
        }

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