using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Models;

namespace Battles.Application.Services.Matches.Commands
{
    public class ReadyMatchCommand : IRequest<BaseResponse>
    {
        public int MatchId { get; set; }
        public string UserId { get; set; }
    }

    public class ReadyMatchCommandHandler : IRequestHandler<ReadyMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;

        public ReadyMatchCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(ReadyMatchCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
            {
                return new BaseResponse("Match not found.", false);
            }

            var user = match.GetUser(request.UserId);

            if (!user.CanLockIn)
            {
                return new BaseResponse("Not allowed to lock in", false);
            }

            user.SetGoFlagUpdatePassLock(false, false, false, ready: true);

            if (match.MatchUsers.All(x => x.Ready))
            {
                match.Status = Status.Pending;

                _ctx.Evaluations.Add(new Evaluation()
                {
                    MatchId = match.Id,
                    Match = match,
                    Expiry = DateTime.Now.AddDays(3)
                });
            }

            await _ctx.SaveChangesAsync(cancellationToken);

            return new BaseResponse("Match locked in.", true);
        }
    }
}