using IdentityServer4.EntityFramework.Options;

namespace Identity.API.Factories;

public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
{
    public PersistedGrantDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
        optionsBuilder.UseSqlServer(".", options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));
        return new PersistedGrantDbContext(optionsBuilder.Options, new OperationalStoreOptions());
    }
}
