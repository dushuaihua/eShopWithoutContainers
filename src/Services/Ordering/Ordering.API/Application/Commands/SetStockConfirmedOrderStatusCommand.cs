namespace eShopWithoutContainers.Services.Ordering.API.Application.Commands;

public class SetStockConfirmedOrderStatusCommand : IRequest<bool>
{
    [DataMember]
    public int OrderNumber { get; set; }

    public SetStockConfirmedOrderStatusCommand(int orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
