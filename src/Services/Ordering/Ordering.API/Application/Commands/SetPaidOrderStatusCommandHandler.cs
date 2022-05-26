namespace eShopWithoutContainers.Services.Ordering.API.Application.Commands;

public class SetPaidOrderStatusCommandHandler : IRequestHandler<SetPaidOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(SetPaidOrderStatusCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(10000, cancellationToken);

        var orderToUpdate = await _orderRepository.GetAsync(request.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetPaidStatus();
        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

public class SetPaidIdentifiedOrderStatusCommandHandler : IdentifiedCommandHandler<SetPaidOrderStatusCommand, bool>
{
    public SetPaidIdentifiedOrderStatusCommandHandler(IMediator mediator, IRequestManager requestManager, ILogger<IdentifiedCommandHandler<SetPaidOrderStatusCommand, bool>> logger) : base(mediator, requestManager, logger)
    {
    }

    protected override bool CreateReusltForDuplicateRequest()
    {
        return true;
    }
}