using System.Collections.Generic;
using System.Linq;
using Battles.Application.Extensions;
using Battles.Application.ViewModels.Matches;
using Battles.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Matches.Queries
{
    public class GetAnonMatchesQuery : IRequest<IEnumerable<MatchViewModel>>
    {
        public int Index { get; set; }
    }

    public class GetAnonMatchesQueryHandler : RequestHandler<GetAnonMatchesQuery, IEnumerable<MatchViewModel>>
    {
        private readonly AppDbContext _ctx;

        public GetAnonMatchesQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override IEnumerable<MatchViewModel> Handle(GetAnonMatchesQuery request)
        {
            return _ctx.Matches
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .Include(x => x.Videos)
                .Where(x => x.Status == Status.Active || x.Status == Status.Complete)
                .OrderByDate()
                .GrabSegment(request.Index)
                .Select(MatchViewModel.ProjectionForAnon)
                .ToList();
        }
    }
}