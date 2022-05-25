namespace eShopWithoutContainers.Services.Ordering.API.Application.Commands;
using static eShopWithoutContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;

public class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftCommand, OrderDraftDTO>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    public CreateOrderDraftCommandHandler(IMediator mediator, IIdentityService identityService)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<OrderDraftDTO> Handle(CreateOrderDraftCommand request, CancellationToken cancellationToken)
    {
        var order = Order.NewDraft();
        var orderItems = request.Items.Select(i => i.ToOrderItemDTO());
        foreach (var item in orderItems)
        {
            order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
        }

        return Task.FromResult(OrderDraftDTO.FromOrder(order));
    }
}

public record OrderDraftDTO
{
    public IEnumerable<OrderItemDTO> OrderItems { get; init; }
    public decimal Total { get; init; }

    public static OrderDraftDTO FromOrder(Order order)
    {
        return new OrderDraftDTO
        {
            OrderItems = order.OrderItems.Select(item => new OrderItemDTO
            {
                Discount = item.Discount,
                ProductId = item.ProductId,
                UnitPrice = item.UnitPrice,
                PictureUrl = item.PictureUrl,
                Units = item.Units,
                ProductName = item.ProductName
            }),
            Total = order.Total
        };
    }
}