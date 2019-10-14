using Battles.Api.Infrastructure;
using Battles.Application.Services.Matches.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Battles.Api.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AnonController : BaseController
    {
        [HttpGet("matches")]
        public async Task<IActionResult> MatchListAnon(int index)
        {
            var matches = await Mediator.Send(new GetAnonMatchesQuery(index));

            return Ok(matches);
        }
        
        [HttpGet("matches/{id}")]
        public async Task<IActionResult> MatchAnon(int id)
        {
            var match = await Mediator.Send(new GetMatchQuery {MatchId = id});

            if (match == null)
                return NoContent();

            return Ok(match);
        }

        [HttpGet("{index}/test")]
        public string Test([FromServices] IHostingEnvironment env)
        {
            return $"Test Message -4, Env: {env.EnvironmentName}";
        }
    }
}