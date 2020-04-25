using TrickingRoyal.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Battles.Application.ViewModels;
using TrickingRoyal.Database.Queries;

namespace Battles.Application.Services.Users.Queries
{
    public class UserNameAvailableQuery : BaseRequest, IRequest<Response<bool>>
    {
        public string DisplayName { get; set; }
    }

    public class UserNameExistsHandler : RequestHandler<UserNameAvailableQuery, Response<bool>>
    {
        private readonly AppDbContext _ctx;

        public UserNameExistsHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override Response<bool> Handle(UserNameAvailableQuery request)
        {
            var nameTaken = _ctx.NameTaken(request.DisplayName, request.UserId);

            return Response.Ok(!nameTaken) ;
        }
    }
}