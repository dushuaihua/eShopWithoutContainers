namespace EventBusServiceBus;
public interface IServiceBusPersisterConnection
{
    ITopicClient TopicClient { get; }
    ISubscriptionClient SubscriptionClient { get; }
}