namespace eShopWithoutContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStartedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; init; }

    public OrderStartedIntegrationEvent(string userId) => UserId = userId;
}
