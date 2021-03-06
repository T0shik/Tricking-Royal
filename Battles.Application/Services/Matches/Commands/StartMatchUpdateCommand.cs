﻿using System;
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
    public class StartMatchUpdateCommand : UpdateSettings, IRequest<BaseResponse>
    {
        public double Start { get; set; }
        public double End { get; set; }
        
        public bool VideoUpdate { get; set; }
    }

    public class StartMatchUpdateCommandHandler : IRequestHandler<StartMatchUpdateCommand, BaseResponse>
    {
        private readonly IMediator _mediator;
        private readonly AppDbContext _ctx;
        private readonly UpdateMatchQueue _matchQueue;
        private readonly ITranslator _translator;

        public StartMatchUpdateCommandHandler(
            IMediator mediator,
            AppDbContext ctx,
            UpdateMatchQueue matchQueue,
            ITranslator translator)
        {
            _mediator = mediator;
            _ctx = ctx;
            _matchQueue = matchQueue;
            _translator = translator;
        }

        public async Task<BaseResponse> Handle(
            StartMatchUpdateCommand command,
            CancellationToken cancellationToken)
        {
            var match = await _ctx.Matches
                                  .Include(x => x.MatchUsers)
                                  .ThenInclude(x => x.User)
                                  .FirstOrDefaultAsync(x => x.Id == command.MatchId,
                                                       cancellationToken: cancellationToken);

            if (match == null)
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "NotFound"));

            if (!match.CanGo(command.UserId))
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "CantGo"));

            _matchQueue.QueueUpdate(command);
            match.Updating = true;
            await _ctx.SaveChangesAsync(cancellationToken);

            return BaseResponse.Ok(await _translator.GetTranslation("Match", "UpdateStarted"));
        }
    }
}