using Battles.Api.Infrastructure;
using Battles.Application.Services.Comments.Commands;
using Battles.Application.Services.Comments.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Battles.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CommentsController : BaseController
    {
        [HttpGet("")]
        public async Task<IActionResult> GetComments([FromQuery] GetCommentsQuery query)
        {
            var comments = await Mediator.Send(query);

            return Ok(comments);
        }

        [HttpGet("sub")]
        public async Task<IActionResult> GetSubComments([FromQuery] GetSubCommentsQuery query)
        {
            var comments = await Mediator.Send(query);

            return Ok(comments);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command)
        {
            command.UserId = UserId;
            var response = await Mediator.Send(command);

            return Ok(response);
        }

        [HttpPost("sub")]
        public async Task<IActionResult> CreateSubComment([FromBody] CreateSubCommentCommand command)
        {
            command.UserId = UserId;
            var response = await Mediator.Send(command);

            return Ok(response);
        }
    }
}