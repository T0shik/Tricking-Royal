using TrickingRoyal.Database;
using Battles.Configuration;
using MediatR;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Helpers;

namespace Battles.Application.Services.Users.Commands
{
    public class UpdateUserPictureCommand : IRequest<BaseResponse>
    {
        [Required] public string Picture { get; set; }

        public string UserId { get; set; }
    }

    public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly Routing _routing;

        public UpdateUserPictureCommandHandler(AppDbContext ctx, Routing routing)
        {
            _ctx = ctx;
            _routing = routing;
        }

        public async Task<BaseResponse> Handle(UpdateUserPictureCommand request, CancellationToken cancellationToken)
        {
            var user = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
                return new BaseResponse("Failed to find user", false);

            user.Picture = CdnUrlHelper.CreateImageUrl(_routing.Cdn, user.Id, request.Picture);

            await _ctx.SaveChangesAsync(cancellationToken);

            return new BaseResponse(user.Picture, true);
        }
    }
}