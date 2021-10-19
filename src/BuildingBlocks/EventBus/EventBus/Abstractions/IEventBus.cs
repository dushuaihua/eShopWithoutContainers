using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void SubScribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void SubScribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
    }
}
