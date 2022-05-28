namespace Ordering.UnitTests.Application;
public class OrdersWebApiTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IOrderQueries> _ordersQueriesMock;
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<ILogger<OrdersController>> _loggerMock;

    public OrdersWebApiTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _ordersQueriesMock = new Mock<IOrderQueries>();
        _identityServiceMock = new Mock<IIdentityService>();
        _loggerMock = new Mock<ILogger<OrdersController>>();
    }

    [Fact]
    public async Task Cancel_order_with_requestId_success()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken))).Returns(Task.FromResult(true));

        var orderController = new OrdersController(_mediatorMock.Object, _ordersQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.CancelOrderAsync(new CancelOrderCommand(1), Guid.NewGuid().ToString()) as OkResult;

        Assert.Equal(actionResult.StatusCode, (int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancel_order_bad_request()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken))).Returns(Task.FromResult(true));

        var orderController = new OrdersController(_mediatorMock.Object, _ordersQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.CancelOrderAsync(new CancelOrderCommand(1), string.Empty) as BadRequestResult;

        Assert.Equal(actionResult.StatusCode, (int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Ship_order_with_requestId_success()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<ShipOrderCommand, bool>>(), default(CancellationToken))).Returns(Task.FromResult(true));

        var orderController = new OrdersController(_mediatorMock.Object, _ordersQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.ShipOrderAsync(new ShipOrderCommand(1), Guid.NewGuid().ToString()) as OkResult;

        Assert.Equal(actionResult.StatusCode, (int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Ship_order_bad_request()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<ShipOrderCommand, bool>>(), default(CancellationToken))).Returns(Task.FromResult(true));

        var orderController = new OrdersController(_mediatorMock.Object, _ordersQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.ShipOrderAsync(new ShipOrderCommand(1), string.Empty) as BadRequestResult;

        Assert.Equal(actionResult.StatusCode, (int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_orders_success()
    {
        var fakeDynamicResult = Enumerable.Empty<OrderSummary>();
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(Guid.NewGuid().ToString());
        _ordersQueriesMock.Setup(x => x.GetOrdersFromUserAsync(Guid.NewGuid())).Returns(Task.FromResult(fakeDynamicResult));

        var orderController = new OrdersController(_mediatorMock.Object, _ordersQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.GetOrdersAsync();

        Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_order_success()
    {
        var fakeOrderId = 123;
        var fakeDynamicResult = new eShopWithoutContainers.Services.Ordering.API.Application.Queries.Order();
        _ordersQueriesMock.Setup(x => x.GetOrderAsync(It.IsAny<int>())).Returns(Task.FromResult(fakeDynamicResult));

        var orderController = new OrdersController(_mediatorMock.Object, _ordersQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.GetOrderAsync(fakeOrderId) as OkObjectResult;

        Assert.Equal(actionResult.StatusCode, (int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_cardTypes_success()
    {
        var fakeDynamicResult = Enumerable.Empty<CardType>();
        _ordersQueriesMock.Setup(x => x.GetCardTypesAsync()).Returns(Task.FromResult(fakeDynamicResult));

        var orderController = new OrdersController(_mediatorMock.Object, _ordersQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.GetCardTypesAsync();

        Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)HttpStatusCode.OK);
    }
}
