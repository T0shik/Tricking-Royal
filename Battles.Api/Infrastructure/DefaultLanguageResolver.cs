using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Transmogrify;

namespace Battles.Api.Infrastructure
{
    public class DefaultLanguageResolver : ILanguageResolver
    {
        private readonly HttpContext _httpContext;

        public DefaultLanguageResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor?.HttpContext;
        }

        public Task<string> GetLanguageCode()
        {
            if (_httpContext != null && _httpContext.Request.Headers.TryGetValue("Accept-Language", out var lang))
            {
                return Task.FromResult(lang.First());
            }

            return Task.FromResult("");
        }
    }
}