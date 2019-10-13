using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Battles.Application.Services.Platform.Queries
{
    public class GetRankingQuery : IRequest<IEnumerable<UserViewModel>>
    {
        public string UserId { get; set; }
    }

    public class GetRankingHandler : RequestHandler<GetRankingQuery, IEnumerable<UserViewModel>>
    {
        private AppDbContext _ctx;

        public GetRankingHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override IEnumerable<UserViewModel> Handle(GetRankingQuery request)
        {
            var users = _ctx.UserInformation
                .Select(UserViewModel.Projection)
                .ToList();
            
            //apply perspective

            return users;
        }
    }
}
