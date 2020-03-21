using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Users.Queries;
using Battles.Enums;
using Transmogrify;

namespace Battles.Application.Services.Users.Commands
{
    public class UpdateUserCommand : IRequest<BaseResponse>
    {
        public string UserId { get; set; }
        [Required] [StringLength(15)] public string DisplayName { get; set; }

        [Required] public int Skill { get; set; }

        [StringLength(255)] public string Information { get; set; }

        [StringLength(30)] public string City { get; set; }
        [StringLength(30)] public string Country { get; set; }
        [StringLength(40)] public string Gym { get; set; }

        [StringLength(100)] public string Instagram { get; set; }
        [StringLength(100)] public string Facebook { get; set; }
        [StringLength(100)] public string Youtube { get; set; }
        public string Language { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly IMediator _mediator;
        private readonly Library _library;

        public UpdateUserCommandHandler(
            AppDbContext ctx,
            IMediator mediator,
            Library library)
        {
            _ctx = ctx;
            _mediator = mediator;
            _library = library;
        }

        public async Task<BaseResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var user = _ctx.UserInformation
                           .FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
                return BaseResponse.Fail(translationContext.Read("User", "NotFound"));

            var newDisplayName = request.DisplayName.Replace(" ", "_");

            var nameAvailable = await _mediator.Send(new UserNameAvailableQuery
            {
                DisplayName = newDisplayName,
                UserId = request.UserId
            }, cancellationToken);

            if (!nameAvailable)
                return BaseResponse.Fail(translationContext.Read("User", "UsernameTaken"));

            user.DisplayName = newDisplayName;
            user.Skill = (Skill) request.Skill;
            user.City = request.City;
            user.Country = request.Country;
            user.Gym = request.Gym;
            user.Information = request.Information;
            user.Instagram = request.Instagram;
            user.Facebook = request.Facebook;
            user.Youtube = request.Youtube;
            user.Language = request.Language;

            if (_ctx.ChangeTracker.HasChanges())
                await _ctx.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(translationContext.Read("User", "ProfileSaved"));
        }
    }
}