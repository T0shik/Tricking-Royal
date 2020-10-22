using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Extensions;
using TrickingRoyal.Database.Extensions;

namespace Battles.Application.Services.Comments.Queries
{
    public class GetSubCommentsQuery : IRequest<IEnumerable<CommentViewModel>>
    {
        public int CommentId { get; set; }
        public int Index { get; set; }
    }

    public class GetSubCommentsQueryHandler : IRequestHandler<GetSubCommentsQuery, IEnumerable<CommentViewModel>>
    {
        private AppDbContext _ctx;

        public GetSubCommentsQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<CommentViewModel>> Handle(GetSubCommentsQuery request, CancellationToken cancellationToken) =>
            await _ctx.SubComments
                .Where(x => x.CommentId == request.CommentId)
                .GrabSegment(request.Index)
                .Select(CommentViewModel.SubCommentProjection)
                .ToListAsync(cancellationToken: cancellationToken);
    }
}
