using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Transmogrify;

namespace Battles.Application.Services.Matches.Commands
{
    public class DeleteMatchCommand : IRequest<BaseResponse>
    {
        public int MatchId { get; set; }
        public string UserId { get; set; }
    }

    public class DeleteMatchCommandHandler : IRequestHandler<DeleteMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly Library _library;

        public DeleteMatchCommandHandler(
            AppDbContext ctx,
            Library library)
        {
            _ctx = ctx;
            _library = library;
        }

        public async Task<BaseResponse> Handle(DeleteMatchCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var match = await _ctx.Matches
                                  .Include(x => x.MatchUsers)
                                  .ThenInclude(x => x.User)
                                  .FirstAsync(x => x.Id == request.MatchId, cancellationToken: cancellationToken);

            if (!match.CanClose(request.UserId))
                return BaseResponse.Fail(translationContext.Read("Match", "CantDelete"));

            var host = match.GetHost();
            if(host == null)
                return BaseResponse.Fail(translationContext.Read("Match", "CantDelete"));
            
            host.User.Hosting--;
            _ctx.Matches.Remove(match);

            await _ctx.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(translationContext.Read("Match", "Deleted"));
        }
    }
}