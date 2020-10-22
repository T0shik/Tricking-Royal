using System.Collections;
using System.Collections.Generic;
using Battles.Api.Infrastructure;
using Battles.Application.Services.Comments.Commands;
using Battles.Application.Services.Comments.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Battles.Application.ViewModels;
using MediatR;

namespace Battles.Api.Controllers
{
    [Authorize]
    public class CommentsController : BaseController
    {
        public CommentsController(IMediator mediator)
            : base(mediator) { }

        [HttpGet]
        public Task<IEnumerable<CommentViewModel>> GetComments([FromQuery] GetCommentsQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpGet("sub")]
        public Task<IEnumerable<CommentViewModel>> GetSubComments([FromQuery] GetSubCommentsQuery query)
        {
            return Mediator.Send(query);
        }

        [HttpPost("")]
        public Task<Response<CommentViewModel>> CreateComment([FromBody] CreateCommentCommand command)
        {
            return Mediator.Send(command);
        }

        [HttpPost("sub")]
        public Task<Response<CommentViewModel>> CreateSubComment([FromBody] CreateSubCommentCommand command)
        {
            return Mediator.Send(command);
        }
    }
}