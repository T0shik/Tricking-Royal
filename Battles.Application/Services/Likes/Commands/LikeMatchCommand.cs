using TrickingRoyal.Database;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Domain.Models;
using Battles.Models;
using Microsoft.EntityFrameworkCore;

namespace Battles.Application.Services.Likes.Commands
{
    public class LikeMatchCommand : IRequest<BaseResponse>
    {
        public int MatchId { get; set; }

        public string UserId { get; set; }
    }

    public class LikeMatchCommandHandler : IRequestHandler<LikeMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;

        public LikeMatchCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(LikeMatchCommand request, CancellationToken cancellationToken)
        {
            if (_ctx.Likes.Any(x => x.MatchId == request.MatchId && x.UserId == request.UserId))
            {
                return new BaseResponse("Already lit.", true);
            }

            var users = _ctx.MatchUser
                .Include(x => x.User)
                .Where(x => x.MatchId == request.MatchId)
                .Select(x => x.User)
                .ToList();

            users.ForEach(x => x.Style++);

            _ctx.Likes.Add(new Like
            {
                MatchId = request.MatchId,
                UserId = request.UserId
            });

            await _ctx.SaveChangesAsync(cancellationToken);

            return new BaseResponse("Match lit.", true);
        }
    }
}