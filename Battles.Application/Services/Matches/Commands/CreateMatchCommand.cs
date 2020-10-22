using System;
using TrickingRoyal.Database;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Microsoft.EntityFrameworkCore;
using Transmogrify;

namespace Battles.Application.Services.Matches.Commands
{
    public class CreateMatchCommand : MatchCreationContext.MatchSettings, IRequest<Response>
    {
    }

    public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, Response>
    {
        private readonly MatchCreationContext _matchCreationContext;
        private readonly AppDbContext _ctx;
        private readonly Library _library;

        public CreateMatchCommandHandler(
            MatchCreationContext matchCreationContext,
            AppDbContext ctx,
            Library library)
        {
            _matchCreationContext = matchCreationContext;
            _ctx = ctx;
            _library = library;
        }

        public async Task<Response> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            request.Host = await _ctx.UserInformation
                .FirstAsync(x => x.Id == request.UserId, cancellationToken);

            try
            {
                var match = _matchCreationContext.CreateMatch(request);
                await _ctx.Matches.AddAsync(match, cancellationToken);
            }
            catch (Exception e)
            {
                return Response.Fail(e.Message);
            }

            await _ctx.SaveChangesAsync(cancellationToken);
            return Response.Ok(translationContext.Read("Match", "Created"));
        }
    }
}