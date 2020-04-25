using TrickingRoyal.Database;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Helpers;
using Battles.Shared;
using Transmogrify;

namespace Battles.Application.Services.Users.Commands
{
    public class UpdateUserPictureCommand : BaseRequest, IRequest<Response>
    {
        [Required] public string Picture { get; set; }
    }

    public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, Response>
    {
        private readonly AppDbContext _ctx;
        private readonly Routing _routing;
        private readonly Library _library;

        public UpdateUserPictureCommandHandler(
            AppDbContext ctx,
            Routing routing,
            Library library)
        {
            _ctx = ctx;
            _routing = routing;
            _library = library;
        }

        public async Task<Response> Handle(UpdateUserPictureCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var user = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
                return Response.Fail(translationContext.Read("User", "NotFound"));

            user.Picture = CdnUrlHelper.CreateImageUrl(_routing.Cdn, user.Id, request.Picture);

            await _ctx.SaveChangesAsync(cancellationToken);

            return Response.Ok(translationContext.Read("User", "PictureUpdated"), user.Picture);
        }
    }
}