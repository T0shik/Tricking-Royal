using IdentityServer.Models.HomeViewModels;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Battles.Shared;

namespace IdentityServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly Routing _routing;

        public HomeController(
            IOptions<OAuth> options,
            IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
            _routing = options.Value.Routing;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View("Index", Path.Combine(_routing.Client, "battles"));
        }

        [HttpGet("privacy")]
        public IActionResult Privacy() => View();

        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}