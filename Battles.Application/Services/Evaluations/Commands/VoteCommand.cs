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
    public class VoteCommand : BaseRequest, IRequest<Response>
    {
        public int EvaluationId { get; set; }
        public int Vote { get; set; }
    }

    public class VoteCommandHandler : IRequestHandler<VoteCommand, Response>
    {
        private readonly AppDbContext _ctx;
        private readonly Library _library;

        public VoteCommandHandler(
            AppDbContext ctx,
            Library library)
        {
            _ctx = ctx;
            _library = library;
        }

        public async Task<Response> Handle(VoteCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();
            var evaluation = _ctx.Evaluations.CanVote(request.EvaluationId, request.UserId);

            if (evaluation == null)
            {
                return Response.Fail(translationContext.Read("Evaluation", "NotAllowed"));
            }

            var user = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
            {
                return Response.Fail(translationContext.Read("User", "NotFound"));
            }

            evaluation.Decisions.Add(new Decision
            {
                EvaluationId = request.EvaluationId,
                Vote = request.Vote,
                Weight = user.Reputation > 0 ? user.VotingPower : 1,
                UserId = request.UserId,
            });

            await _ctx.SaveChangesAsync(cancellationToken);

            return Response.Ok(translationContext.Read("Evaluation", "VoteCreated"));
        }
    }
}