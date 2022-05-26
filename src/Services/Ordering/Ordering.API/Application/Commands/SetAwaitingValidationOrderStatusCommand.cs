namespace eShopWithoutContainers.Services.Ordering.API.Application.Commands;

public class SetAwaitingValidationOrderStatusCommand : IRequest<bool>
{
    public int OrderNumber { get; private set; }

    public SetAwaitingValidationOrderStatusCommand(int orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
