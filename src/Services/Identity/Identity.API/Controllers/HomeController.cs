namespace Identity.API.Controllers;
public class HomeController : Controller
{
    private readonly IIdentityServerInteractionService _interactionService;
    private readonly IWebHostEnvironment _environment;

    public HomeController(IIdentityServerInteractionService interactionService, IWebHostEnvironment environment)
    {
        _interactionService = interactionService;
        _environment = environment;
    }

    public IActionResult Index()
    {
        if (!_environment.IsProduction())
        {
            return View();
        }
        return NotFound();
    }


    public async Task<IActionResult> Error(string errorId)
    {
        var model = new ErrorViewModel();

        var message = await _interactionService.GetErrorContextAsync(errorId);

        if (message is not null)
        {
            model.Error = message;
        }
        return View("Error", model);
    }
}