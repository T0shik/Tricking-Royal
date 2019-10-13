using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Battles.Cdn.Infrastructure
{
    public class BaseController : Controller
    {
        protected string UserId =>
            HttpContext.User.Claims.Where(c => c.Type == "sub")
                .Select(c => c.Value)
                .FirstOrDefault();
    }
}