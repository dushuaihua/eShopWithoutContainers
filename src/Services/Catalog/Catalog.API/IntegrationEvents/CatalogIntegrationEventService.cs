namespace eShopWithoutContainers.Services.Catalog.API.IntegrationEvents;
public class CatalogIntegrationEventService : ICatalogIntegrationEventService, IDisposable
{
    private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
    private readonly IEventBus _eventBus;
    private readonly CatalogContext _catalogContext;
    private readonly IIntegrationEventLogService _integrationEventLogService;
    private readonly ILogger<CatalogIntegrationEventService> _logger;
    private volatile bool disposedValue;

    public CatalogIntegrationEventService(
        ILogger<CatalogIntegrationEventService> logger,
        IEventBus eventBus,
        CatalogContext catalogContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
        _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _integrationEventLogService = _integrationEventLogServiceFactory(_catalogContext.Database.GetDbConnection());
    }

    public async Task PublishThroughEventBusAsync(IntegrationEvent @event)
    {
        try
        {
            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
            await _integrationEventLogService.MarkEventAsInProgressAsync(@event.Id);
            _eventBus.Publish(@event);
            await _integrationEventLogService.MarkEventAsPublishedAsync(@event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event:{IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
            await _integrationEventLogService.MarkEventAsFailedAsync(@event.Id);
        }
    }

    public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent @event)
    {
        _logger.LogInformation("----- CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", @event.Id);
        await ResilientTransaction.New(_catalogContext).ExecuteAsync(async () =>
        {
            await _catalogContext.SaveChangesAsync();
            await _integrationEventLogService.SaveEventAsync(@event, _catalogContext.Database.CurrentTransaction);
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                (_integrationEventLogService as IDisposable)?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}