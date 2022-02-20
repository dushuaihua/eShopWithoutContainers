using IdentityServer4.EntityFramework.Options;

namespace Identity.API.Factories;

public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
{
    public ConfigurationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
        optionsBuilder.UseSqlServer(".", options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));
        return new ConfigurationDbContext(optionsBuilder.Options, new ConfigurationStoreOptions());
    }
}
