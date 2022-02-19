using eShopWithoutContainers.BuildingBlocks.EventBus.Abstractions;
using System.Threading.Tasks;

namespace eShopWithoutContainers.BuildingBlocks.EventBus.Tests
{
    public class TestIntegrationOtherEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; set; }
        public TestIntegrationOtherEventHandler()
        {
            Handled = false;
        }
        public async Task Handle(TestIntegrationEvent @event)
        {
            Handled = true;
        }
    }
}
