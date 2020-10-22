using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Battles.Api.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public string Test([FromServices] IHostingEnvironment env)
        {
            return $"Test Message -4, Env: {env.EnvironmentName}";
        }
    }
}