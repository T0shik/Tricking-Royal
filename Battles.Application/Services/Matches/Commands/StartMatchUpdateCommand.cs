using System.Threading;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Rules.Matches.Actions.Update;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transmogrify;
using TrickingRoyal.Database;

namespace Battles.Application.Services.Matches.Commands
{
    public class StartMatchUpdateCommand : UpdateSettings, IRequest<Response>
    {
        public double Start { get; set; }
        public double End { get; set; }

        public bool VideoUpdate { get; set; }
    }

    public class StartMatchUpdateCommandHandler : IRequestHandler<StartMatchUpdateCommand, Response>
    {
        private readonly IMediator _mediator;
        private readonly AppDbContext _ctx;
        private readonly UpdateMatchQueue _matchQueue;
        private readonly Library _library;

        public StartMatchUpdateCommandHandler(
            IMediator mediator,
            AppDbContext ctx,
            UpdateMatchQueue matchQueue,
            Library library)
        {
            _mediator = mediator;
            _ctx = ctx;
            _matchQueue = matchQueue;
            _library = library;
        }

        public async Task<Response> Handle(
            StartMatchUpdateCommand command,
            CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            var match = await _ctx.Matches
                                  .Include(x => x.MatchUsers)
                                  .FirstOrDefaultAsync(x => x.Id == command.MatchId,
                                                               cancellationToken: cancellationToken);

            if (match == null)
                return Response.Fail(translationContext.Read("Match", "NotFound"));

            if (!match.CanGo(command.UserId))
                return Response.Fail(translationContext.Read("Match", "CantGo"));

            _matchQueue.QueueUpdate(command);
            match.Updating = true;
            await _ctx.SaveChangesAsync(cancellationToken);

            return Response.Ok(translationContext.Read("Match", "UpdateStarted"));
        }
    }
}