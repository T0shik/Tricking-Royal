using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Rules.Evaluations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Evaluations.Queries
{
    public class GetVoteResultsQuery : IRequest<DecisionResultViewModel>
    {
        public string UserId { get; set; }
        public int EvaluationId { get; set; }
    }

    public class GetVoteResultsQueryHandler : IRequestHandler<GetVoteResultsQuery, DecisionResultViewModel>
    {
        private readonly AppDbContext _ctx;

        public GetVoteResultsQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<DecisionResultViewModel> Handle(GetVoteResultsQuery request,
            CancellationToken cancellationToken)
        {
            var decisions = await _ctx.Decisions
                .Where(x => x.EvaluationId == request.EvaluationId)
                .ToListAsync(cancellationToken: cancellationToken);

            var results = ResultsCalculator.Calculate(decisions);

            return new DecisionResultViewModel
            {
                HostVotes = results.GetHostVotes(),
                HostPercent = results.GetHostPercent(),
                OpponentVotes = results.GetOpponentVotes(),
                OpponentPercent = results.GetOpponentPercent(),
                Winner = (int) results.GetWinner(),
            };
        }
    }
}