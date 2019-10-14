using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Battles.Enums;

namespace Battles.Application.Services.Platform.Queries
{
    public class GetPlatformInfoQuery : IRequest<PlatformStatsViewModel>
    {
        public string RealUserId { get; set; }
    }

    public class GetPlatformInfoHandler : RequestHandler<GetPlatformInfoQuery, PlatformStatsViewModel>
    {
        private AppDbContext _ctx;

        public GetPlatformInfoHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override PlatformStatsViewModel Handle(GetPlatformInfoQuery request)
        {
            var stats = new PlatformStatsViewModel
            {
                Users = _ctx.UserInformation.Count(),
                Matches = _ctx.Matches.Count(),
                Open = _ctx.Matches.Where(m => m.Status == Status.Open).Count(),
                Tribunal = _ctx.Evaluations.Where(e => !e.Complete).Count()
            };

            return stats;
        }
    }
}
