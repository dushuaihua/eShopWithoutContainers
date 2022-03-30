namespace eShopWithoutContainers.Services.Ordering.Domain.Events;

public class OrderStatusChangedToStockConfirmedDomainEvent : INotification
{
    public int OrderId { get; }
    public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
    {
        OrderId = orderId;
    }
}
