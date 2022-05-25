
namespace eShopWithoutContainers.Services.Ordering.API.Infrastructure.AutofacModules;

public class MediatorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

        //builder.RegisterAssemblyTypes(typeof(CreateOrderCommand))
    }
}
