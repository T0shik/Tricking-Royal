using System;
using TrickingRoyal.Database;
using Battles.Rules.Matches.Actions.Create;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Battles.Application.Services.Matches.Commands
{
    public class CreateMatchCommand : MatchSettings, IRequest<BaseResponse>
    {
        public string UserId { get; set; }
    }

    public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;

        public CreateMatchCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
        {
            request.Host = await _ctx.UserInformation
                                     .FirstAsync(x => x.Id == request.UserId, cancellationToken);

            try
            {
                var match = MatchCreator.CreateMatch(request);
                _ctx.Matches.Add(match);
            }
            catch (Exception e)
            {
                return new BaseResponse(e.Message, false);
            }

            await _ctx.SaveChangesAsync(cancellationToken);
            return new BaseResponse("Match Created", true);
        }
    }
}