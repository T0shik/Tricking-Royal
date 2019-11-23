using System;
using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Models;

namespace Battles.Application.Services.Evaluations.Commands
{
    public class CloseFlagEvaluationCommand : IRequest<Unit>
    {
        public CloseFlagEvaluationCommand(Evaluation e)
        {
            Evaluation = e;
        }

        public Evaluation Evaluation { get; set; }
    }

    public class CloseFlagEvaluationCommandHandler : IRequestHandler<CloseFlagEvaluationCommand, Unit>
    {
        private readonly AppDbContext _ctx;

        public CloseFlagEvaluationCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Unit> Handle(CloseFlagEvaluationCommand request, CancellationToken cancellationToken)
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
            
            //todo: this needs it's own calculator

            //if (VoteResults.Winner() <= 0)
            //{
            //    evaluation.Match.Status = Status.Closed;
            //    evaluation.Match.Finished = DateTime.Now.GetFinishTime();

            //    Host.User.Hosting--;
            //    Opponent.User.Joined--;

            //    if (evaluation.Match.IsTurn(MatchRole.Host))
            //    {
            //        Opponent.SetPenalty(10, true);
            //        Host.SetLockUser();
            //    }
            //    else if (evaluation.Match.IsTurn(MatchRole.Opponent))
            //    {
            //        Host.SetPenalty(10, true);
            //        Opponent.SetLockUser();
            //    }
            //}
            //else if (VoteResults.Winner() > 0)
            //{
            //    evaluation.Match.Status = Status.Active;
            //    if (evaluation.Match.IsTurn(MatchRole.Host))
            //    {
            //        Host.SetPenalty(10);
            //        Host.SetGoFlagUpdatePassLock(true, false, false);
            //    }
            //    else if (evaluation.Match.IsTurn(MatchRole.Opponent))
            //    {
            //        Opponent.SetPenalty(10);
            //        Opponent.SetGoFlagUpdatePassLock(true, false, false);
            //    }
            //}
            //else
            //{
            //    evaluation.Match.Status = Status.Active;
            //}

            evaluation.Complete = true;

            evaluation.Decisions
                //.Where(x => x.Vote == VoteResults.Winner())
                .Select(x => x.User)
                .ToList()
                .ForEach(x => { x.Reputation += 2; });

            await _ctx.SaveChangesAsync(cancellationToken);

            return new Unit();
        }
    }
}