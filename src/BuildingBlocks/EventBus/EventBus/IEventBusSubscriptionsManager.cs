namespace EventBus;
public interface IEventBusSubscriptionsManager
{
    bool IsEmpty { get; }
    event EventHandler<string> OnEventRemoved;
    void AddDynamicSubScription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;
    void AddSubsciption<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
    void RemoveSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
    void RemoveDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler;
    bool HasSubscriptionsForEvent<T>()
        where T : IntegrationEvent;
    bool HasSubscriptionsForEvent(string eventName);
    Type GetEventTypeByName(string eventName);
    void Clear();
    IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    string GetEventKey<T>();
}
