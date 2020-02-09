using System;
using TrickingRoyal.Database;
using Battles.Rules.Matches.Actions.Create;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Microsoft.EntityFrameworkCore;
using Transmogrify;

namespace Battles.Application.Services.Matches.Commands
{
    public class CreateMatchCommand : MatchSettings, IRequest<BaseResponse>
    {
        public string UserId { get; set; }
    }

    public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly ITranslator _translator;

        public CreateMatchCommandHandler(AppDbContext ctx, ITranslator translator)
        {
            _ctx = ctx;
            _translator = translator;
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
            return BaseResponse.Ok(await _translator.GetTranslation("Match", "Created"));
        }

    }
}