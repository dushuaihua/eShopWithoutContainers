namespace eShopWithoutContainers.BuildingBlocks.EventBus.Abstractions;
public interface IIntegrationEventHandler { }
public interface IIntegrationEventHandler<in TIntegratioinEvent> : IIntegrationEventHandler
    where TIntegratioinEvent : IntegrationEvent
{
    Task Handle(TIntegratioinEvent @event);
}
