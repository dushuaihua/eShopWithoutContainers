namespace eShopWithoutContainers.Services.Identity.API.Controllers;

public class HomeController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IOptionsSnapshot<AppSettings> _settings;
    private readonly IRedirectService _redirectService;

    public HomeController(IIdentityServerInteractionService interaction, IOptionsSnapshot<AppSettings> settings, IRedirectService redirectService)
    {
        _interaction = interaction;
        _settings = settings;
        _redirectService = redirectService;
    }

    public IActionResult Index(string returnUrl)
    {
        return View();
    }

    public IActionResult ReturnToOriginalApplication(string returnUrl)
    {
        if (returnUrl != null)
        {
            return Redirect(_redirectService.ExtractRedirectUriFromReturnUrl(returnUrl));
        }
        else
        {
            return RedirectToAction("index", "home");
        }
    }

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
