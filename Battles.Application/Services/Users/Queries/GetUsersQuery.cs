using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Battles.Application.Services.Users.Queries
{
    public class GetUsersQuery : IRequest<IEnumerable<UserViewModel>>
    {
        public string Search { get; set; }
    }

    public class GetUsersHandler : RequestHandler<GetUsersQuery, IEnumerable<UserViewModel>>
    {
        private AppDbContext _ctx;

        public GetUsersHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override IEnumerable<UserViewModel> Handle(GetUsersQuery request) =>
            _ctx.UserInformation
                    .Where(x => EF.Functions.Like(x.DisplayName, $"%{request.Search}%"))
                    .Select(UserViewModel.Projection)
                    .ToList();
    }
}
