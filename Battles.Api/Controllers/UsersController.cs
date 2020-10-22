using System.Collections.Generic;
using Battles.Api.Infrastructure;
using Battles.Application.Services.Users.Commands;
using Battles.Application.Services.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Battles.Application;
using Battles.Application.ViewModels;
using MediatR;

namespace Battles.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(IMediator mediator)
            : base(mediator) { }

        [HttpGet("")]
        public Task<IEnumerable<UserViewModel>> GetUsers([FromQuery] GetUsersQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpGet("{displayName}")]
        public Task<UserViewModel> GetUser([FromRoute] GetUserQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpGet("{displayName}/can-use")]
        public Task<Response<bool>> CanUseDisplayName([FromRoute] UserNameAvailableQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpPost("")]
        public Task<UserViewModel> CreateUser([FromBody] ActivateUserCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPut("")]
        public Task<Response> UpdateUser([FromBody] UpdateUserCommand command)
        {
            return ModelState.IsValid
                       ? Mediator.Send(command)
                       : Task.FromResult(Application.ViewModels.Response.Fail("Invalid information submitted"));
        }

        [HttpPut("picture")]
        public Task<Response> UpdateUserPicture([FromBody] UpdateUserPictureCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPut("level-up")]
        public Task<Response> UpdateUserPicture([FromBody] LevelUpCommand command)
        {
            return  Mediator.Send(command);
        }

        [HttpPut("language")]
        public Task<Response> UpdateUserLanguage([FromBody] UpdateUserLanguageCommand command)
        {
            return Mediator.Send(command);
        }
    }
}