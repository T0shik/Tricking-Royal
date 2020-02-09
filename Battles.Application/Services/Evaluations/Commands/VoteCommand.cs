using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Extensions;
using Battles.Models;
using Transmogrify;

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
        private readonly ITranslator _translator;

        public VoteCommandHandler(
            AppDbContext ctx, 
            ITranslator translator)
        {
            _ctx = ctx;
            _translator = translator;
        }

        public async Task<BaseResponse> Handle(VoteCommand request, CancellationToken cancellationToken)
        {
            var evaluation = _ctx.Evaluations.CanVote(request.EvaluationId, request.UserId);

            if (evaluation == null)
            {
                return BaseResponse.Fail(await _translator.GetTranslation("Evaluation", "NotAllowed"));
            }

            var user = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
            {
                return BaseResponse.Fail(await _translator.GetTranslation("User", "NotFound"));
            }

            evaluation.Decisions.Add(new Decision
            {
                EvaluationId = request.EvaluationId,
                Vote = request.Vote,
                Weight = user.Reputation > 0 ? user.VotingPower : 1,
                UserId = request.UserId,
            });

            await _ctx.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(await _translator.GetTranslation("Evaluation", "VoteCreated"));
        }
    }
}