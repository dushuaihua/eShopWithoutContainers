using Identity.API.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Controllers
{
    [Authorize]
    public class GrantsController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private IEventService _eventService;

        public GrantsController(IIdentityServerInteractionService interactionService, IClientStore clientStore, IResourceStore resourceStore, IEventService eventService)
        {
            _interactionService = interactionService;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            return View("Index", await BuildViewModelAsync());
        }

        private async Task<GrantsViewModel> BuildViewModelAsync()
        {
            var grants = await _interactionService.GetAllUserGrantsAsync();

            var list = new List<GrantViewModel>();

            foreach (var grant in grants)
            {
                var client = await _clientStore.FindClientByIdAsync(grant.ClientId);
                if (client is not null)
                {
                    var resources = await _resourceStore.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantViewModel
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Description = grant.Description,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            return new GrantsViewModel
            {
                Grants = list
            };
        }
    }
}
