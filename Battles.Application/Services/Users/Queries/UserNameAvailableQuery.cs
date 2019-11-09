using TrickingRoyal.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace Battles.Application.Services.Users.Queries
{
    public class UserNameAvailableQuery : IRequest<bool>
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }

    public class UserNameExistsHandler : RequestHandler<UserNameAvailableQuery, bool>
    {
        private AppDbContext _ctx;

        public UserNameExistsHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override bool Handle(UserNameAvailableQuery request)
        {
            var someoneElseHasName = _ctx.UserInformation
                    .Any(x => x.DisplayName == request.DisplayName 
                        && x.Id != request.UserId);

            return !someoneElseHasName;
        }
    }
}
