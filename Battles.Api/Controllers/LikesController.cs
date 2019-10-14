using Battles.Api.Infrastructure;
using Battles.Application.Services.Likes.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Battles.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [Produces("application/json")]
    public class LikesController : BaseController
    {
        [HttpPost("{id}")]
        public async Task<IActionResult> LikeMatch(int id)
        {
            var result = await Mediator.Send(new LikeMatchCommand { MatchId = id, UserId = UserId });

            return Ok(result);
        }
    }
}
