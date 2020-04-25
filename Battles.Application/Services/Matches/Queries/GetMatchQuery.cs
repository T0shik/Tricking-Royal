using System.Linq;
using Battles.Application.ViewModels.Matches;
using Battles.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Matches.Queries
{
    public class GetMatchQuery : BaseRequest, IRequest<MatchViewModel>
    {
        public int MatchId { get; set; }
    }

    public class GetMatchQueryHandler : RequestHandler<GetMatchQuery, MatchViewModel>
    {
        private readonly AppDbContext _ctx;

        public GetMatchQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override MatchViewModel Handle(GetMatchQuery request)
        {
            if (string.IsNullOrEmpty(request.UserId)) return GetMatchQuery(request.MatchId);

            var match = MatchQuery()
                .Include(x => x.Comments)
                .ThenInclude(x => x.SubComments)
                .FirstOrDefault(x => x.Id == request.MatchId);

            return MatchViewModel.GetMatchWithComments(match, request.UserId);
        }

        private MatchViewModel GetMatchQuery(int matchId)
        {
            return MatchQuery()
                .Select(MatchViewModel.ProjectionForAnon)
                .FirstOrDefault(x => x.Id == matchId);
        }

        private IQueryable<Match> MatchQuery()
        {
            return _ctx.Matches
                .Include(x => x.Videos)
                .Include(x => x.Likes)
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User);
        }
    }
}