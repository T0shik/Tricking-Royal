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
using Transmogrify;

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
        private readonly ITranslator _translator;

        public FlagMatchCommandHandler(
            AppDbContext ctx,
            ITranslator translator)
        {
            _ctx = ctx;
            _translator = translator;
        }

        public async Task<BaseResponse> Handle(FlagMatchCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
            {
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "NotFound"));
            }

            if (!match.CanFlag(request.UserId))
            {
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "CantFlag"));
            }

            match.Status = Status.Pending;
            foreach (var user in match.MatchUsers)
            {
                user.SetFreeze(true);
            }

            _ctx.Evaluations.Add(new Evaluation
            {
                Match = match,
                EvaluationType = EvaluationT.Flag,
                Reason = request.Reason,
                Expiry = DateTime.Now.AddDays(1),
            });

            await _ctx.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(await _translator.GetTranslation("Match", "Flagged"));
        }
    }
}