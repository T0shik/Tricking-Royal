using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;
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

        public LevelUpCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public async Task<BaseResponse> Handle(LevelUpCommand request, CancellationToken cancellationToken)
        {
            var user = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
            {
                return new BaseResponse("Failed to find user record.", false);
            }
            
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
                    return new BaseResponse("Invalid perk selected.", false);
            }

            user.LevelUpPoints--;

            await _ctx.SaveChangesAsync(cancellationToken);
            
            return new BaseResponse("Leveled Up!", true);
        }
    }

    public enum PerkType
    {
        Host,
        Join,
        Vote,
    }
}