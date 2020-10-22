using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Battles.Api.Infrastructure
{
    public class AppendUserIdPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppendUserIdPipelineBehaviour(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (request is BaseRequest br)
            {
                br.UserId = _httpContextAccessor.HttpContext.User.Claims
                                                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                                                ?.Value;
            }

            return next();
        }
    }
}