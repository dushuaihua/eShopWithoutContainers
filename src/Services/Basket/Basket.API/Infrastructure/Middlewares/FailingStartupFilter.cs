namespace eShopWithoutContainers.Services.Basket.API.Infrastructure.Middlewares;
public class FailingStartupFilter : IStartupFilter
{
    private Action<FailingOptions> _options;

    public FailingStartupFilter(Action<FailingOptions> optionsAction)
    {
        _options = optionsAction;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseFailingMiddleware(_options);
            next(app);
        };
    }
}