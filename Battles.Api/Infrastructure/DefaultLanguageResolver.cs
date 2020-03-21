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
            _httpContext = httpContextAccessor.HttpContext;
        }
        
        public Task<string> GetLanguageCode()
        {
            return Task.FromResult(_httpContext.Request.Headers.TryGetValue("Accept-Language", out var lang) ? lang.First() : "");
        }
    }
}