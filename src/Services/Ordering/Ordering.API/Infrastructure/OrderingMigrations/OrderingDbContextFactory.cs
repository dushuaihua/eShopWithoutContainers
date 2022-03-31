namespace eShopWithoutContainers.Services.Ordering.API.Infrastructure.OrderingMigrations;

public class OrderingDbContextFactory : IDesignTimeDbContextFactory<OrderingContext>
{
    public OrderingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderingContext>();

        optionsBuilder.UseSqlServer(".", options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));

        return new OrderingContext(optionsBuilder.Options);
    }
}
