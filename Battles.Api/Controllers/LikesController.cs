using Battles.Api.Infrastructure;
using Battles.Application.Services.Likes.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;

namespace Battles.Api.Controllers
{
    [Authorize]
    public class LikesController : BaseController
    {
        public LikesController(IMediator mediator)
            : base(mediator) { }

        [HttpPost("{matchId}")]
        public Task<Response> LikeMatch([FromRoute] LikeMatchCommand command)
        {
            return Mediator.Send(command);
        }
    }
}