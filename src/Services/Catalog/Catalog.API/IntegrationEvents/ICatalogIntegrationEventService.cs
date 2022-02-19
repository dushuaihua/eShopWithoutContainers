namespace eShopWithoutContainers.Services.Catalog.API.IntegrationEvents;
public interface ICatalogIntegrationEventService
{
    Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent @event);
    Task PublishThroughEventBusAsync(IntegrationEvent @event);
}