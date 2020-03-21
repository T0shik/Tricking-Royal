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
        [Required] [StringLength(15)] public string DisplayName { get; set; }

        [Required] public int Skill { get; set; }

        [StringLength(255)] public string Information { get; set; }

        [StringLength(30)] public string City { get; set; }
        [StringLength(30)] public string Country { get; set; }
        [StringLength(40)] public string Gym { get; set; }

        [StringLength(100)] public string Instagram { get; set; }
        [StringLength(100)] public string Facebook { get; set; }
        [StringLength(100)] public string Youtube { get; set; }

        public string UserId { get; set; }

        public UpdateUserCommand AttachUserId(string id)
        {
            UserId = id;
            return this;
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly IMediator _mediator;
        private readonly ITranslator _translation;

        public UpdateUserCommandHandler(
            AppDbContext ctx,
            IMediator mediator,
            ITranslator translation)
        {
            _ctx = ctx;
            _mediator = mediator;
            _translation = translation;
        }

        public async Task<BaseResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _ctx.UserInformation
                           .FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
                return BaseResponse.Fail(await _translation.GetTranslation("User", "NotFound"));

            var newDisplayName = request.DisplayName.Replace(" ", "_");

            var nameAvailable = await _mediator.Send(new UserNameAvailableQuery
            {
                DisplayName = newDisplayName,
                UserId = request.UserId
            }, cancellationToken);

            if (!nameAvailable)
                return BaseResponse.Fail(await _translation.GetTranslation("User", "UsernameTaken"));

            user.DisplayName = newDisplayName;
            user.Skill = (Skill) request.Skill;
            user.City = request.City;
            user.Country = request.Country;
            user.Gym = request.Gym;
            user.Information = request.Information;
            user.Instagram = request.Instagram;
            user.Facebook = request.Facebook;
            user.Youtube = request.Youtube;

            if (_ctx.ChangeTracker.HasChanges())
                await _ctx.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(await _translation.GetTranslation("User", "ProfileSaved"));
        }
    }
}