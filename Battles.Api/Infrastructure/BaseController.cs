using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Battles.Api.Infrastructure
{
    public class BaseController : Controller
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());

        protected string UserId =>
            HttpContext.User.Claims.Where(c => c.Type == "sub")
                .Select(c => c.Value)
                .FirstOrDefault();

        protected string UserEmail =>
            HttpContext.User.Claims.Where(c => c.Type == "email")
                .Select(c => c.Value)
                .FirstOrDefault();
    }
}