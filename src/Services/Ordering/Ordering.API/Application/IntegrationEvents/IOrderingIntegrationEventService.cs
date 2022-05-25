namespace eShopWithoutContainers.Services.Ordering.API.Application.IntegrationEvents;

public interface IOrderingIntegrationEventService
{
    Task PubishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(IntegrationEvent @event);
}