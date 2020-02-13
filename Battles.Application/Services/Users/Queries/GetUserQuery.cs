using Battles.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Rules.Levels;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Users.Queries
{
    public class GetUserQuery : IRequest<UserViewModel>
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserViewModel>
    {
        private readonly AppDbContext _ctx;

        public GetUserQueryHandler(AppDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<UserViewModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var query = _ctx.UserInformation.AsQueryable();

            query = request.DisplayName == "me" 
                ? query.Where(x => x.Id == request.UserId)
                : query.Where(x => x.DisplayName == request.DisplayName);

            var user = await query
                .Select(UserViewModel.Projection)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            user.ExperienceNeed = LevelSystem.ExpNeededForLevelUp(user.Level);
            
            return user;
        }
    }
}
