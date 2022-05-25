namespace eShopWithoutContainers.Services.Ordering.API.Application.IntegrationEvents;

public class OrderingIntegrationEventService : IOrderingIntegrationEventService
{
    private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
    private readonly IEventBus _eventBus;
    private readonly OrderingContext _orderingContext;
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly ILogger<OrderingIntegrationEventService> _logger;


    public OrderingIntegrationEventService(IEventBus eventBus,
        OrderingContext orderingContext,
        IntegrationEventLogContext eventLogContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
        ILogger<OrderingIntegrationEventService> logger)
    {
        _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
        _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _eventLogService = _integrationEventLogServiceFactory(_orderingContext.Database.GetDbConnection());
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PubishEventsThroughEventBusAsync(Guid transactionId)
    {
        var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

        foreach (var pendingLogEvent in pendingLogEvents)
        {
            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", pendingLogEvent.EventId, Program.AppName, pendingLogEvent.IntegrationEvent);

            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(pendingLogEvent.EventId);
                _eventBus.Publish(pendingLogEvent.IntegrationEvent);
                await _eventLogService.MarkEventAsPublishedAsync(pendingLogEvent.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", pendingLogEvent.EventId, Program.AppName);

                await _eventLogService.MarkEventAsFailedAsync(pendingLogEvent.EventId);
            }
        }
    }

    public async Task AddAndSaveEventAsync(IntegrationEvent @event)
    {
        _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", @event.Id, @event);

        await _eventLogService.SaveEventAsync(@event, _orderingContext.GetCurrentTransaction());
    }
}
