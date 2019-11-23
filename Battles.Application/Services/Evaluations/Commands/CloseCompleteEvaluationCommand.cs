using TrickingRoyal.Database;
using Battles.Domain.Models;
using Battles.Rules.Matches.Extensions;
using MediatR;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Extensions;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Evaluations;
using Battles.Rules.Levels;

namespace Battles.Application.Services.Evaluations.Commands
{
    public class CloseCompleteEvaluationCommand : IRequest<Unit>
    {
        public CloseCompleteEvaluationCommand(Evaluation e)
        {
            Evaluation = e;
        }

        public Evaluation Evaluation { get; set; }
    }

    public class CloseCompleteEvaluationCommandHandler : IRequestHandler<CloseCompleteEvaluationCommand, Unit>
    {
        private readonly AppDbContext _ctx;

        public CloseCompleteEvaluationCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Unit> Handle(CloseCompleteEvaluationCommand request,
            CancellationToken cancellationToken)
        {
            var evaluation = request.Evaluation;

            if (evaluation.Decisions.Count <= 0)
            {
                evaluation.Expiry = DateTime.Now.AddDays(1);
                await _ctx.SaveChangesAsync(cancellationToken);

                return new Unit();
            }

            var host = evaluation.Match.GetHost();
            var opponent = evaluation.Match.GetOpponent();

            var calcResult = ResultsCalculator.Calculate(evaluation.Decisions);
            var winner = calcResult.GetWinner();

            if (winner == Winner.Draw)
            {
                host.SetDraw(10).AwardExp(10);
                opponent.SetDraw(10).AwardExp(10);
            }
            else if (winner == Winner.Host)
            {
                host.SetWinner(10).AwardExp(12);
                opponent.SetLoser(10).AwardExp(8);
            }
            else if (winner == Winner.Opponent)
            {
                host.SetLoser(10).AwardExp(8);
                opponent.SetWinner(10).AwardExp(12);
            }

            host.User.Hosting--;
            opponent.User.Joined--;

            evaluation.Complete = true;

            evaluation.Match.Status = Status.Complete;
            evaluation.Match.LastUpdate = DateTime.Now;
            evaluation.Match.Finished = evaluation.Match.LastUpdate.GetFinishTime();

            evaluation.Decisions
                .Select(x => x.User)
                .ToList()
                .ForEach(user =>
                {
                    user.Reputation += 2;
                    user.AwardExp(2);
                });

            await _ctx.SaveChangesAsync(cancellationToken);

            return new Unit();
        }
    }
}