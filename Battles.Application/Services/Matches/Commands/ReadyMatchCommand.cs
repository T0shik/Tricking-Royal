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
using Transmogrify;

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
        private readonly Library _library;

        public ReadyMatchCommandHandler(AppDbContext ctx, Library library)
        {
            _ctx = ctx;
            _library = library;
        }

        public async Task<BaseResponse> Handle(ReadyMatchCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
                return BaseResponse.Fail(translationContext.Read("Match", "NotFound"));

            var user = match.GetUser(request.UserId);

            if (!user.CanLockIn)
                return BaseResponse.Fail(translationContext.Read("Match", "CantLockIn"));

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

            return BaseResponse.Ok(translationContext.Read("Match", "LockedIn"));
        }
    }
}