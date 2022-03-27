namespace eShopWithoutContainers.Services.Payment.API.IntegrationEvents.Events;

public record OrderPaymentFaildIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentFaildIntegrationEvent(int orderId) => OrderId = orderId;
}
