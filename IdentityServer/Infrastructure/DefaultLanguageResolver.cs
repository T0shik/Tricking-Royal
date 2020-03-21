using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Transmogrify;

namespace IdentityServer.Infrastructure
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
            if (_httpContext.Request.Cookies.TryGetValue("lang", out var lang))
                return Task.FromResult(lang);

            lang = GetLanguage();
            
            if (!string.IsNullOrEmpty(lang))
                _httpContext.Response.Cookies.Append("lang", lang);
            
            return Task.FromResult(lang);
        }

        public string GetLanguage()
        {
            if (_httpContext.Request.Headers.TryGetValue("Accept-Language", out var lang))
                return lang.First();
            if (_httpContext.Request.Query.TryGetValue("lang", out lang))
                return lang.First();
            return "";
        }
    }
}