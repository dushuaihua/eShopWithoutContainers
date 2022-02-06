namespace Identity.API.Controllers;
[Authorize]
public class DeviceController : Controller
{
    private readonly IDeviceFlowInteractionService _interactionService;
    private readonly IEventService _eventService;
    private readonly IOptions<IdentityServerOptions> _options;

    public DeviceController(
        IDeviceFlowInteractionService interactionService,
        IEventService eventService,
        IOptions<IdentityServerOptions> options)
    {
        _interactionService = interactionService;
        _eventService = eventService;
        _options = options;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string userCodeParamName = _options.Value.UserInteraction.DeviceVerificationUserCodeParameter;
        string userCode = Request.Query[userCodeParamName];

        if (string.IsNullOrWhiteSpace(userCode))
        {
            return View("UserCodeCapture");
        }

        var model = await BuildViewModelAsync(userCode);

        if (model is null)
        {
            return View("Error");
        }

        model.ConfirmUserCode = true;
        return View("UserCodeConfirmation", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserCodeCapture(string userCode)
    {
        var model = await BuildViewModelAsync(userCode);
        if (model is null)
        {
            return View("Error");
        }
        return View("UserCodeConfirmation", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Callback(DeviceAuthorizationInputModel model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        var result = await ProcessConsent(model);

        if (result.HasValidationError)
        {
            return View("Error");
        }
        return View("Success");
    }


    private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInputModel model)
    {
        var result = new ProcessConsentResult();

        var request = await _interactionService.GetAuthorizationContextAsync(model.UserCode);

        if (request is null)
        {
            return result;
        }

        ConsentResponse grantedConsent = null;

        if (model.Button == "no")
        {
            grantedConsent = new ConsentResponse
            {
                Error = AuthorizationError.AccessDenied
            };

            await _eventService.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
        }
        else if (model.Button == "yes")
        {
            if (model.ScopesConsented is not null && model.ScopesConsented.Any())
            {
                var scopes = model.ScopesConsented;
                if (ConsentOptions.EnableOfflineAccess == false)
                {
                    scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);
                }

                grantedConsent = new ConsentResponse
                {
                    RememberConsent = model.RememberConsent,
                    ScopesValuesConsented = scopes.ToArray(),
                    Description = model.Description
                };

                await _eventService.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
            }
            else
            {
                result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
            }
        }
        else
        {
            result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
        }

        if (grantedConsent is not null)
        {
            await _interactionService.HandleRequestAsync(model.UserCode, grantedConsent);

            result.RedirectUri = model.ReturnUrl;
            result.Client = request.Client;
        }
        else
        {
            result.ViewModel = await BuildViewModelAsync(model.UserCode, model);
        }

        return result;
    }

    private async Task<DeviceAuthorizationViewModel> BuildViewModelAsync(string userCode, DeviceAuthorizationInputModel model = null)
    {
        var request = await _interactionService.GetAuthorizationContextAsync(userCode);

        if (request is not null)
        {
            return CreateConsentViewModel(userCode, model, request);
        }

        return null;
    }

    private DeviceAuthorizationViewModel CreateConsentViewModel(string userCode, DeviceAuthorizationInputModel model, DeviceFlowAuthorizationRequest request)
    {
        var viewModel = new DeviceAuthorizationViewModel
        {
            UserCode = userCode,
            Description = model?.Description,
            RememberConsent = model?.RememberConsent ?? true,
            ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            AllowRememberConsent = request.Client.AllowRememberConsent
        };

        viewModel.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, viewModel.ScopesConsented.Contains(x.Name) || model is null)).ToArray();

        var apiScopes = new List<ScopeViewModel>();
        foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
        {
            var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);

            if (apiScope is not null)
            {
                var scopeViewMode = CreateScopeViewModel(parsedScope, apiScope, viewModel.ScopesConsented.Contains(parsedScope.RawValue) || model is null);

                apiScopes.Add(scopeViewMode);
            }
        }

        if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
        {
            apiScopes.Add(GetOfflineAccessScope(viewModel.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) || model is null));
        }

        return viewModel;
    }

    private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    {
        return new ScopeViewModel
        {
            Value = identity.Name,
            DisplayName = identity.DisplayName ?? identity.Name,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required
        };
    }

    private ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        return new ScopeViewModel
        {
            Value = parsedScopeValue.RawValue,
            DisplayName = apiScope.DisplayName ?? apiScope.Name,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required
        };
    }

    private ScopeViewModel GetOfflineAccessScope(bool check)
    {
        return new ScopeViewModel
        {
            Value = IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescrption,
            Emphasize = true,
            Checked = check
        };
    }
}