namespace eShopWithoutContainers.Services.Identity.API;
public class Startup
{
    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        RegisterAppInsights(services);

        //Add framework services.
        services.AddDbContext<ApplicationDbContext>
    }

    private void RegisterAppInsights(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry(Configuration);
        services.AddApplicationInsightsKubernetesEnricher();
    }
}
