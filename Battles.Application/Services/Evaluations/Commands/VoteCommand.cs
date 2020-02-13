using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Extensions;
using Battles.Models;

namespace Battles.Application.Services.Evaluations.Commands
{
    public class VoteCommand : IRequest<BaseResponse>
    {
        public int EvaluationId { get; set; }
        public int Vote { get; set; }
        public string UserId { get; set; }
    }

    public class VoteCommandHandler : IRequestHandler<VoteCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;

        public VoteCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(VoteCommand request, CancellationToken cancellationToken)
        {
            var evaluation = _ctx.Evaluations.CanVote(request.EvaluationId, request.UserId);

            //todo stick this in evaluation exceptions
            if (evaluation == null)
            {
                return new BaseResponse("Not allowed to vote.", false);
            }

            var user = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
            {
                return new BaseResponse("User not found.", false);
            }

            evaluation.Decisions.Add(new Decision
            {
                EvaluationId = request.EvaluationId,
                Vote = request.Vote,
                Weight = user.Reputation > 0 ? user.VotingPower : 1,
                UserId = request.UserId,
            });

            await _ctx.SaveChangesAsync(cancellationToken);

            return new BaseResponse("Vote submitted.", true);
        }
    }
}