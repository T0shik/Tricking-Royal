using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;

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

        public DeleteMatchCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(DeleteMatchCommand request, CancellationToken cancellationToken)
        {
            var match = await _ctx.Matches
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .FirstAsync(x => x.Id == request.MatchId, cancellationToken: cancellationToken);

            if (!match.CanClose(request.UserId))
                return new BaseResponse("Can't delete match", false);

            var host = match.GetHost();
            if(host == null)
                return new BaseResponse("Can't delete match", false);
            
            host.User.Hosting--;
            _ctx.Matches.Remove(match);

            await _ctx.SaveChangesAsync(cancellationToken);

            return new BaseResponse("Match deleted.", true);
        }
    }
}