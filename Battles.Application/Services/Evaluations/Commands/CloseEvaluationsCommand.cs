using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Evaluations;
using Battles.Rules.Evaluations.Actions.Close;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Evaluations.Commands
{
    public class CloseEvaluationsCommand : IRequest<Unit> { }

    public class CloseEvaluationsCommandHandler : IRequestHandler<CloseEvaluationsCommand, Unit>
    {
        private readonly AppDbContext _ctx;

        public CloseEvaluationsCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Unit> Handle(CloseEvaluationsCommand request, CancellationToken cancellationToken)
        {
            foreach (var evaluation in GetExpiredEvaluations())
            {
                if (evaluation.Decisions.Count <= 0)
                {
                    evaluation.Expiry = DateTime.Now.AddDays(1);
                }
                else
                {
                    EvaluationActionFactory.CreateClose(evaluation).Close();
                }

                await _ctx.SaveChangesAsync(cancellationToken);
            }

            return new Unit();
        }

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