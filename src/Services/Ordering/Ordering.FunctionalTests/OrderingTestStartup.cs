namespace Ordering.FunctionalTests;

public class OrderingTestStartup : Startup
{
    public OrderingTestStartup(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void ConfigureAuth(IApplicationBuilder app)
    {
        if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
        {
            app.UseMiddleware<AutoAuthorizeMiddleware>();
        }
        else
        {
            base.ConfigureAuth(app);
        }
    }

    public override IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.Configure<RouteOptions>(Configuration);
        return base.ConfigureServices(services);
    }
}