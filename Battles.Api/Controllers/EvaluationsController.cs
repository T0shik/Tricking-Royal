using Battles.Api.Infrastructure;
using Battles.Application.Services.Evaluations.Commands;
using Battles.Application.Services.Evaluations.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Battles.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [Produces("application/json")]
    public class EvaluationsController : BaseController
    {
        [HttpGet("")]
        public async Task<IActionResult> GetEvaluations(string type)
        {
            var query = new GetEvaluationsQuery {UserId = UserId, Type = type};
            var evaluations = await Mediator.Send(query);

            return Ok(evaluations);
        }

        [HttpGet("{evaluationId}/results")]
        public async Task<IActionResult> GetResult([FromRoute] GetVoteResultsQuery query)
        {
            query.UserId = UserId;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetEvaluationCount()
        {
            var evaluations = await Mediator.Send(new GetEvaluationCountQuery {UserId = UserId});

            return Ok(evaluations);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> MakeDecision(int id, [FromBody] VoteCommand command)
        {
            command.EvaluationId = id;
            command.UserId = UserId;

            var result = await Mediator.Send(command);

            return Ok(result);
        }
    }
}