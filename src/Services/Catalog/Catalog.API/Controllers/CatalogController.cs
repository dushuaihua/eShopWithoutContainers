using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly CatalogSettings _settings;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;
    }
}
