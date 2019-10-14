using System.Data;
using Battles.Api.Infrastructure;
using Battles.Application.Services.Users.Commands;
using Battles.Application.Services.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Battles.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UsersController : BaseController
    {
        [HttpGet("")]
        public async Task<IActionResult> GetUsers([FromQuery] string searchString)
        {
            var result = await Mediator.Send(new GetUsersQuery {Search = searchString});

            return Ok(result);
        }

        [HttpGet("{displayName}")]
        public async Task<IActionResult> GetUser(string displayName)
        {
            var user = await Mediator.Send(new GetUserQuery
            {
                DisplayName = displayName,
                UserId = UserId
            });

            if (user == null)
            {
                return NoContent();
            }

            return Ok(user);
        }

        [HttpGet("{displayName}/can-use")]
        public async Task<IActionResult> CanUseDisplayName(string displayName)
        {
            var result = await Mediator.Send(new UserNameAvailableQuery
            {
                DisplayName = displayName,
                UserId = UserId
            });

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateUser([FromBody] ActivateUserCommand command)
        {
            command.UserId = UserId;

            var result = await Mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            //TODO Validation Messages
            if (!ModelState.IsValid)
                return BadRequest("Invalid information submitted");

            var result = await Mediator.Send(command.AttachUserId(UserId));

            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpPut("picture")]
        public async Task<IActionResult> UpdateUserPicture([FromBody] UpdateUserPictureCommand command)
        {
            command.UserId = UserId;

            var result = await Mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("level-up")]
        public async Task<IActionResult> UpdateUserPicture([FromBody] LevelUpCommand command)
        {
            command.UserId = UserId;

            var result = await Mediator.Send(command);

            return Ok(result);
        }
    }
}