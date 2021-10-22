using Microsoft.Azure.ServiceBus;

namespace EventBusServiceBus
{
    public interface IServiceBusPersisterConnection
    {
        ITopicClient TopicClient { get; }
        ISubscriptionClient SubscriptionClient { get; }
    }
}