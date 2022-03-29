using Microsoft.Extensions.Options;
using Serilog.Context;

namespace eShopWithoutContainers.Services.Payment.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
{
    private readonly IEventBus _eventBus;
    private readonly PaymentSettings _settings;
    private readonly ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> _logger;

    public OrderStatusChangedToStockConfirmedIntegrationEventHandler(IEventBus eventBus, IOptionsSnapshot<PaymentSettings> settings, ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger)
    {
        _eventBus = eventBus;
        _settings = settings.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogTrace("PaymentSettings: {@PaymentSettings}", _settings);
    }

    public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            IntegrationEvent orderPaymentIntegrationEvent;

            if (_settings.PaymentSucceeded)
            {
                orderPaymentIntegrationEvent = new OrderPaymentSucceededIntegrationEvent(@event.OrderId);
            }
            else
            {
                orderPaymentIntegrationEvent = new OrderPaymentFaildIntegrationEvent(@event.OrderId);
            }

            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", orderPaymentIntegrationEvent.Id, Program.AppName, orderPaymentIntegrationEvent);

            _eventBus.Publish(orderPaymentIntegrationEvent);

            await Task.CompletedTask;
        }
    }
}
