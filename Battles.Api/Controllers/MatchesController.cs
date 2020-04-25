using System.Collections.Generic;
using Battles.Api.Infrastructure;
using Battles.Application.Services.Matches.Commands;
using Battles.Application.Services.Matches.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using Battles.Application.ViewModels.Matches;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Api.Controllers
{
    [Authorize]
    public class MatchesController : BaseController
    {
        public MatchesController(IMediator mediator)
            : base(mediator) { }

        [HttpGet]
        public Task<IEnumerable<object>> MatchList([FromQuery] GetMatchesQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpGet("{matchId}")]
        public Task<MatchViewModel> Match([FromRoute] GetMatchQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpDelete("{matchId}")]
        public Task<Response> DeleteMatch([FromRoute] DeleteMatchCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPut("{matchId}")]
        public Task<Response> JoinMatch([FromRoute] JoinMatchCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPut("{matchId}/update")]
        public Task<Response> UpdateMatch(int matchId, [FromBody] StartMatchUpdateCommand command)
        {
            command.MatchId = matchId;
            return Mediator.Send(command);
        }

        [HttpPut("{matchId}/video")]
        public Task<Response> UpdateVideo(int matchId, [FromBody] UpdateMatchVideoCommand command)
        {
            command.MatchId = matchId;
            return Mediator.Send(command);
        }

        [HttpPost("{matchId}/three-round-pass/ready")]
        public Task<Response> ReadyThreeRoundPass([FromRoute] ReadyMatchCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPost("{matchId}/pass")]
        public Task<Response> PassMatch([FromRoute] PassRoundCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPost("{matchId}/forfeit")]
        public Task<Response> ForfeitMatch([FromRoute] ForfeitMatchCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPost("{matchId}/flag")]
        public Task<Response> FlagMatch(int matchId, [FromBody] FlagMatchCommand command)
        {
            command.MatchId = matchId;
            return Mediator.Send(command);
        }

        [HttpPost]
        public Task<Response> CreateMatch([FromBody] CreateMatchCommand command)
        {
            return Mediator.Send(command);
        }
    }
}