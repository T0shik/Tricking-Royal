using Battles.Api.Infrastructure;
using Battles.Api.Workers.Notifications.Settings;
using Battles.Application.Services.Platform.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Battles.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class PlatformController : BaseController
    {
        [AllowAnonymous]
        [HttpGet("platform")]
        public IActionResult PlatformStats()
        {
            var stats = Mediator.Send(new GetPlatformInfoQuery());

            return Ok(stats);
        }

        [HttpGet("ranking")]
        public IActionResult Ranking()
        {
            var ranking = Mediator.Send(new GetRankingQuery());

            return Ok(ranking);
        }

        [HttpGet("one-signal")]
        public IActionResult OneSignalAppId([FromServices] IOptionsMonitor<OneSignal> options)
        {
            return Ok(new {options.CurrentValue.AppId, options.CurrentValue.SafariId});
        }
    }
}