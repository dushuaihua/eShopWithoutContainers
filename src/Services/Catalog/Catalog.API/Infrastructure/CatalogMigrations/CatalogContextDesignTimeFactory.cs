namespace Catalog.API.Infrastructure.IntegrationEventMigrations
{
    public class CatalogContextDesignTimeFactory : IDesignTimeDbContextFactory<CatalogContext>
    {
        public CatalogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(".");

            return new CatalogContext(optionsBuilder.Options);
        }
    }
}
