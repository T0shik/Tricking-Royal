using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Battles.Api.Infrastructure;
using Battles.Api.Workers.Notifications.Settings;
using Battles.Application.Services.Platform.Queries;
using Battles.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Battles.Api.Controllers
{
    [Authorize]
    public class PlatformController : BaseController
    {
        public PlatformController(IMediator mediator)
            : base(mediator) { }

        [AllowAnonymous]
        [HttpGet("platform")]
        public Task<PlatformStatsViewModel> PlatformStats()
        {
            return Mediator.Send(new GetPlatformInfoQuery());
        }

        [HttpGet("ranking")]
        public Task<IEnumerable<UserViewModel>> Ranking()
        {
            return Mediator.Send(new GetRankingQuery());
        }

        [HttpGet("one-signal")]
        public IActionResult OneSignalAppId([FromServices] IOptionsMonitor<OneSignal> options)
        {
            return Ok(new {options.CurrentValue.AppId, options.CurrentValue.SafariId});
        }
    }
}