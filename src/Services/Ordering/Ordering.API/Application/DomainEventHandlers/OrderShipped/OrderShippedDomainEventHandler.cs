namespace eShopWithoutContainers.Services.Ordering.API.Application.DomainEventHandlers.OrderShipped;

public class OrderShippedDomainEventHandler : INotificationHandler<OrderShippedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
    private readonly ILoggerFactory _logger;

    public OrderShippedDomainEventHandler(IOrderRepository orderRepository, IBuyerRepository buyerRepository, IOrderingIntegrationEventService orderingIntegrationEventService, ILoggerFactory logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService)); ;
    }

    public async Task Handle(OrderShippedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderShippedDomainEventHandler>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
            notification.Order.Id, nameof(OrderStatus.Shipped), OrderStatus.Shipped.Id);

        var order = await _orderRepository.GetAsync(notification.Order.Id);
        var buyer = await _buyerRepository.FindByIdAsync(order.BuyerId.Value.ToString());

        var orderStatusChangedToShippedIntegrationEvent = new OrderStatusChangedToShippedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
        await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToShippedIntegrationEvent);
    }
}
