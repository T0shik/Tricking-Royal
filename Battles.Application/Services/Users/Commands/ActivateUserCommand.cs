using Battles.Application.ViewModels;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Battles.Enums;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Users.Commands
{
    public class ActivateUserCommand : BaseRequest, IRequest<UserViewModel>
    {
        public int Skill { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<ActivateUserCommand, UserViewModel>
    {
        private readonly AppDbContext _ctx;

        public CreateUserCommandHandler(AppDbContext ctx) =>
            _ctx = ctx;

        public async Task<UserViewModel> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var userInfo = await _ctx.UserInformation
                .FirstAsync(x => x.Id == request.UserId, cancellationToken: cancellationToken);

            userInfo.Id = request.UserId;
            userInfo.HostingLimit = 1;
            userInfo.JoinedLimit = 1;
            userInfo.Activated = true;
            userInfo.Skill = (Skill) request.Skill;

            await _ctx.SaveChangesAsync(cancellationToken);

            return UserViewModel.Projection.Compile().Invoke(userInfo);
        }
    }
}