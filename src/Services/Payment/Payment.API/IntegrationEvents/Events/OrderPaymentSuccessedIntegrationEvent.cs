namespace eShopWithoutContainers.Services.Payment.API.IntegrationEvents.Events;

public record OrderPaymentSuccessedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentSuccessedIntegrationEvent(int orderId) => OrderId = orderId;
}
