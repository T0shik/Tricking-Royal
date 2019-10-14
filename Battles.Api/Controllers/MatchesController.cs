using Battles.Api.Infrastructure;
using Battles.Application.Services.Matches.Commands;
using Battles.Application.Services.Matches.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Battles.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [Produces("application/json")]
    public class MatchesController : BaseController
    {
        [HttpGet("")]
        public async Task<IActionResult> MatchList([FromQuery] GetMatchesQuery query)
        {
            query.UserId = UserId;

            var matches = await Mediator.Send(query);

            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Match(int id)
        {
            var match = await Mediator.Send(new GetMatchQuery {MatchId = id, UserId = UserId});

            if (match == null)
                return NoContent();

            return Ok(match);
        }

        [HttpDelete("{matchId}")]
        public async Task<IActionResult> DeleteMatch(int matchId)
        {
            var response = await Mediator.Send(new DeleteMatchCommand {MatchId = matchId, UserId = UserId});

            return Ok(response);
        }

        [HttpPut("{matchId}")]
        public async Task<IActionResult> JoinMatch(int matchId)
        {
            var result = await Mediator.Send(new JoinMatchCommand {MatchId = matchId, UserId = UserId});

            return Ok(result);
        }

        [HttpPut("{matchId}/update")]
        public async Task<IActionResult> UpdateMatch(int matchId, [FromBody] UpdateMatchCommand command)
        {
            command.MatchId = matchId;
            command.UserId = UserId;

            var response = await Mediator.Send(command);

            return Ok(response);
        }

        [HttpPut("{matchId}/video")]
        public async Task<IActionResult> UpdateVideo(int matchId, [FromBody] UpdateMatchVideoCommand command)
        {
            command.MatchId = matchId;
            command.UserId = UserId;

            var result = await Mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("{id}/three-round-pass/ready")]
        public async Task<IActionResult> ReadyThreeRoundPass(int id)
        {
            var response = await Mediator.Send(new ReadyMatchCommand {MatchId = id, UserId = UserId});

            return Ok(response);
        }

        [HttpPost("{id}/pass")]
        public async Task<IActionResult> PassMatch(int id)
        {
            var result = await Mediator.Send(new PassRoundCommand {MatchId = id, UserId = UserId});

            return Ok(result);
        }

        [HttpPost("{id}/forfeit")]
        public async Task<IActionResult> ForfeitMatch(int id)
        {
            var response = await Mediator.Send(new ForfeitMatchCommand {MatchId = id, UserId = UserId});

            return Ok(response);
        }

        [HttpPost("{id}/flag")]
        public async Task<IActionResult> FlagMatch(int id, [FromBody] FlagMatchCommand command)
        {
            command.MatchId = id;
            command.UserId = UserId;

            var response = await Mediator.Send(command);

            return Ok(response);
        }


        [HttpPost("")]
        public async Task<IActionResult> CreateMatch([FromBody] CreateMatchCommand command)
        {
            command.UserId = UserId;

            var created = await Mediator.Send(command);

            return Ok(created);
        }
    }
}