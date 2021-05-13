using Identity.API.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientStore _clientStore;
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _eventService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
             IIdentityServerInteractionService interactionService,
             IClientStore clientStore,
             ILogger<AccountController> logger,
             IAuthenticationSchemeProvider schemeProvider,
             IEventService eventService,
             UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager)
        {
            _interactionService = interactionService;
            _clientStore = clientStore;
            _logger = logger;
            _schemeProvider = schemeProvider;
            _eventService = eventService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> SignIn(string returnUrl)
        {
            var model = await BuildSignInViewModelAsync(returnUrl);
            if (model.IsExternalSigninOnly)
            {
                return RedirectToAction("Challenge", "External", new { scheme = model.ExternalSignInScheme, returnUrl });
            }
            return View(model);
        }


        public async Task<IActionResult> SignIn(SignInInputModel model, string button)
        {
            var context = await _interactionService.GetAuthorizationContextAsync(model.ReturnUrl);

            if (button != "signin")
            {
                if (context is not null)
                {
                    await _interactionService.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    if (context.IsNativeClient())
                    {
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return Redirect("/");
                }
            }
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.Email, user.Id, user.UserName, clientId: context?.Client.ClientId));

                    if (context is not null)
                    {
                        if (context.IsNativeClient())
                        {
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }
                        return Redirect(model.ReturnUrl);
                    }

                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        throw new Exception("Invalid return URL");
                    }
                }

                await _eventService.RaiseAsync(new UserLoginFailureEvent(model.Email, "Invalid credentials", clientId: context?.Client.ClientId));

                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            var viewModel = await BuildSignInViewModelAsync(model);
            ViewData["ReturnUrl"] = model.ReturnUrl;

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> SignOut(string signOutId)
        {
            var model = await BuildSignOutViewModelAsync(signOutId);
            if (model.ShowSignOutPrompt == false)
            {
                return await SignOut(model);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut(SignOutInputModel model)
        {
            var viewModel = await BuildSignedOutViewModeAsync(model.SignOutId);
            if (User?.Identity.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();

                await _eventService.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }
            if (viewModel.TriggerExternalSignOut)
            {
                var url = Url.Action("SignOut", new { signoutId = viewModel.SignOutId });
                return SignOut(new AuthenticationProperties { RedirectUri = url }, viewModel.ExternalAuthenticationScheme);
            }
            return View("SignedOut", viewModel);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task<SignInViewModel> BuildSignInViewModelAsync(string returnUrl)
        {
            var context = await _interactionService.GetAuthorizationContextAsync(returnUrl);

            if (context?.IdP is not null && await _schemeProvider.GetSchemeAsync(context.IdP) is not null)
            {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;
                var model = new SignInViewModel
                {
                    EnableLocalSignIn = local,
                    ReturnUrl = returnUrl,
                    Email = context?.LoginHint
                };

                if (!local)
                {
                    model.ExternalProviders = new[]
                    {
                        new ExternalProvider{ AuthenticationScheme = context.IdP }
                    };
                }
                return model;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes.Where(x => x.DisplayName is not null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId is not null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client is not null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions is not null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new SignInViewModel
            {
                AllowRememberMe = AccountOptions.AllRememberMe,
                EnableLocalSignIn = allowLocal && AccountOptions.AllowLocalSignIn,
                ReturnUrl = returnUrl,
                Email = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<SignInViewModel> BuildSignInViewModelAsync(SignInInputModel model)
        {
            var viewModel = await BuildSignInViewModelAsync(model.ReturnUrl);
            viewModel.Email = model.Email;
            viewModel.RememberMe = model.RememberMe;
            return viewModel;
        }

        private async Task<SignOutViewModel> BuildSignOutViewModelAsync(string signOutId)
        {
            var model = new SignOutViewModel
            {
                SignOutId = signOutId,
                ShowSignOutPrompt = AccountOptions.ShowSignOutPrompt
            };

            if (User?.Identity.IsAuthenticated != true)
            {
                model.ShowSignOutPrompt = false;
                return model;
            }

            var context = await _interactionService.GetLogoutContextAsync(signOutId);
            if (context?.ShowSignoutPrompt == false)
            {
                model.ShowSignOutPrompt = false;
                return model;
            }
            return model;
        }

        private async Task<SignedOutViewModel> BuildSignedOutViewModeAsync(string signOutId)
        {
            var signOut = await _interactionService.GetLogoutContextAsync(signOutId);
            var model = new SignedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostSignOutRedirectUri = signOut?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(signOut?.ClientName) ? signOut?.ClientId : signOut?.ClientName,
                SignOutIframeUrl = signOut?.SignOutIFrameUrl,
                SignOutId = signOutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp is not null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);

                    if (providerSupportsSignout)
                    {
                        if (model.SignOutId is null)
                        {
                            model.SignOutId = await _interactionService.CreateLogoutContextAsync();
                        }
                    }
                }
            }
            return model;
        }
    }
}
