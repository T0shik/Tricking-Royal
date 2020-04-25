using System.Collections;
using System.Collections.Generic;
using Battles.Api.Infrastructure;
using Battles.Application.Services.Matches.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Battles.Application.ViewModels.Matches;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Battles.Api.Controllers
{
    [AllowAnonymous]
    public class AnonController : BaseController
    {
        public AnonController(IMediator mediator)
            : base(mediator) { }

        [HttpGet("matches")]
        public Task<IEnumerable<MatchViewModel>> MatchListAnon([FromQuery] GetAnonMatchesQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpGet("matches/{id}")]
        public Task<MatchViewModel> MatchAnon([FromRoute] GetMatchQuery query)
        {
            return Mediator.Send(query);
        }
    }
}