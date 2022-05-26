namespace eShopWithoutContainers.Services.Ordering.API.Application.Commands;

public class SetAwaitingValidationOrderStatusCommandHandler : IRequestHandler<SetAwaitingValidationOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public SetAwaitingValidationOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(SetAwaitingValidationOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetAsync(request.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }
        orderToUpdate.SetAwatingValidationStatus();
        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

public class SetAwaitingValidationIdentifiedOrderStatusCommandHandler : IdentifiedCommandHandler<SetAwaitingValidationOrderStatusCommand, bool>
{
    public SetAwaitingValidationIdentifiedOrderStatusCommandHandler(IMediator mediator, IRequestManager requestManager, ILogger<IdentifiedCommandHandler<SetAwaitingValidationOrderStatusCommand, bool>> logger) : base(mediator, requestManager, logger)
    {
    }
    protected override bool CreateReusltForDuplicateRequest()
    {
        return true;
    }
}