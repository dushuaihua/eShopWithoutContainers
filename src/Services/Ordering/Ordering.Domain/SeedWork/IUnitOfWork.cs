namespace eShopWithoutContainers.Services.Ordering.Domain.SeedWork;
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
