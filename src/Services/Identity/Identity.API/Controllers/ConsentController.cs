namespace eShopWithoutContainers.Services.Identity.API.Controllers;

public class ConsentController : Controller
{
    private readonly ILogger<ConsentController> _logger;
    private readonly IClientStore _clientStore;
    private readonly IResourceStore _resourceStore;
    private readonly IIdentityServerInteractionService _interaction;

    public ConsentController(
        ILogger<ConsentController> logger,
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IResourceStore resourceStore)
    {
        _logger = logger;
        _interaction = interaction;
        _clientStore = clientStore;
        _resourceStore = resourceStore;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string returnUrl)
    {
        var vm = await BuildViewModelAsync(returnUrl);
        ViewData["ReturnUrl"] = returnUrl;
        if (vm != null)
        {
            return View("index", vm);
        }
        return View("error");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ConsentInputModel model)
    {
        var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
        ConsentResponse response = null;

        if (model.Button == "no")
        {
            response = new ConsentResponse();
        }
        else if (model.Button == "yes" && model != null)
        {
            if (model.ScopesConsented != null && model.ScopesConsented.Any())
            {
                response = new ConsentResponse
                {
                    RememberConsent = model.RememberConsent,
                    ScopesValuesConsented = model.ScopesConsented
                };
            }
            else
            {
                ModelState.AddModelError("", "You must pick at least one permission.");
            }
        }
        else
        {
            ModelState.AddModelError("", "Invalid Selection");
        }

        if (response != null)
        {
            await _interaction.GrantConsentAsync(request, response);

            return Redirect(model.ReturnUrl);
        }

        var vm = await BuildViewModelAsync(model.ReturnUrl, model);
        if (vm != null)
        {
            return View("index", vm);
        }

        return View("Error");
    }

    private async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
    {
        var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (request != null)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(request.Client.ClientId);
            if (client != null)
            {
                var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ValidatedResources.InvalidScopes);
                if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                {
                    return new ConsentViewModel(model, returnUrl, request, client, resources);
                }
                else
                {
                    _logger.LogError("No scopes matching: {0}", request.ValidatedResources.InvalidScopes.Aggregate((x, y) => x + ", " + y));
                }
            }
            else
            {
                _logger.LogError("Invalid client id: {0}", request.Client.ClientId);
            }
        }
        else
        {
            _logger.LogError("No consent request matching request: {0}", returnUrl);
        }
        return null;
    }
}
