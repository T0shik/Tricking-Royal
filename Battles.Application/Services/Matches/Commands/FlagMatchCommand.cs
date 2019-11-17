using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Models;

namespace Battles.Application.Services.Matches.Commands
{
    public class FlagMatchCommand : IRequest<BaseResponse>
    {
        [Required] public string Reason { get; set; }
        public int MatchId { get; set; }
        public string UserId { get; set; }
    }

    public class FlagMatchCommandHandler : IRequestHandler<FlagMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;

        public FlagMatchCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(FlagMatchCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
            {
                return new BaseResponse("Match not found.", false);
            }

            if (!match.CanFlag(request.UserId))
            {
                return new BaseResponse("Not allowed to flag match.", false);
            }

            match.Status = Status.Pending;

            var e = new Evaluation()
            {
                Match = match,
                EvaluationType = EvaluationT.Flag,
                Reason = request.Reason,
                Expiry = DateTime.Now.AddDays(1)
            };

            _ctx.Matches.Update(match);
            _ctx.Evaluations.Add(e);

            await _ctx.SaveChangesAsync(cancellationToken);

            return new BaseResponse("Match flagged.", true);
        }
    }
}