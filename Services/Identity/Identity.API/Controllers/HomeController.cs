using Identity.API.Models;
using Identity.API.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Identity.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IRedirectService _redirectService;

        public HomeController(IIdentityServerInteractionService interactionService,
            IOptionsSnapshot<AppSettings> settings, IRedirectService redirectService)
        {
            _interactionService = interactionService;
            _settings = settings;
            _redirectService = redirectService;
        }

        public IActionResult Index(string returnUrl)
        {
            return View();
        }

        public IActionResult ReturnToOriginalApplication(string returnUrl)
        {
            if (returnUrl is not null)
            {
                return Redirect(_redirectService.ExtractRedirectUriFromReturnUrl(returnUrl));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            var message = await _interactionService.GetErrorContextAsync(errorId);

            if (message is not null)
            {
                vm.Error = message;
            }
            return View("Error", vm);
        }
    }
}
