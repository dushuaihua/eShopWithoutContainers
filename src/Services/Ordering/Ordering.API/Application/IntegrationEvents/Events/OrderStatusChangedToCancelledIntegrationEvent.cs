namespace eShopWithoutContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToCancelledIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; set; }
    public string OrderStatus { get; set; }
    public string BuyerName { get; }
    public OrderStatusChangedToCancelledIntegrationEvent(int orderId, string orderStatus, string buyerName)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
    }
}
