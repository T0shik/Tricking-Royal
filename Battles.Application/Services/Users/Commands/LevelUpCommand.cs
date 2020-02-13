using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;
using Transmogrify;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Users.Commands
{
    public class LevelUpCommand : IRequest<BaseResponse>
    {
        public int Type { get; set; }
        public string UserId { get; set; }
    }
    
    public class LevelUpCommandHandler : IRequestHandler<LevelUpCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly ITranslator _translation;

        public LevelUpCommandHandler(AppDbContext ctx, ITranslator translation)
        {
            _ctx = ctx;
            _translation = translation;
        }
        
        public async Task<BaseResponse> Handle(LevelUpCommand request, CancellationToken cancellationToken)
        {
            var user = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
                return BaseResponse.Fail(await _translation.GetTranslation("User", "NotFound"));
            
            switch ((PerkType) request.Type)
            {
                case PerkType.Host:
                    user.HostingLimit++;
                    break;
                case PerkType.Join:
                    user.JoinedLimit++;
                    break;
                case PerkType.Vote:
                    user.VotingPower++;
                    break;
                default:
                    return BaseResponse.Fail(await _translation.GetTranslation("User", "InvalidPerk"));
            }

            user.LevelUpPoints--;

            await _ctx.SaveChangesAsync(cancellationToken);
            
            return BaseResponse.Ok(await _translation.GetTranslation("User", "LeveledUp"));
        }
    }

    public enum PerkType
    {
        Host,
        Join,
        Vote,
    }
}