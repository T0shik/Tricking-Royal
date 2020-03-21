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
        private readonly Library _library;

        public LevelUpCommandHandler(AppDbContext ctx, Library library)
        {
            _ctx = ctx;
            _library = library;
        }
        
        public async Task<BaseResponse> Handle(LevelUpCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var user = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
                return BaseResponse.Fail(translationContext.Read("User", "NotFound"));
            
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
                    return BaseResponse.Fail(translationContext.Read("User", "InvalidPerk"));
            }

            user.LevelUpPoints--;

            await _ctx.SaveChangesAsync(cancellationToken);
            
            return BaseResponse.Ok(translationContext.Read("User", "LeveledUp"));
        }
    }

    public enum PerkType
    {
        Host,
        Join,
        Vote,
    }
}