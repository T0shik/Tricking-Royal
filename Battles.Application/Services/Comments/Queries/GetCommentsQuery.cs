using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Extensions;

namespace Battles.Application.Services.Comments.Queries
{
    public class GetCommentsQuery : IRequest<IEnumerable<CommentViewModel>>
    {
        public int MatchId { get; set; }
        public int Index { get; set; }
    }

    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, IEnumerable<CommentViewModel>>
    {
        private AppDbContext _ctx;

        public GetCommentsQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<CommentViewModel>> Handle(GetCommentsQuery request, CancellationToken cancellationToken) =>
            await _ctx.Comments
                .Where(x => x.MatchId == request.MatchId)
                .GrabSegment(request.Index)
                .Select(CommentViewModel.CommentProjection)
                .ToListAsync(cancellationToken: cancellationToken);
    }
}