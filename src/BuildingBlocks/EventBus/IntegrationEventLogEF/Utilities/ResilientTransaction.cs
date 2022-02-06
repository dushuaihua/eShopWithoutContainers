namespace IntegrationEventLogEF.Utilities;
public class ResilientTransaction
{
    private DbContext _context;

    public ResilientTransaction(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public static ResilientTransaction New(DbContext context)
    {
        return new ResilientTransaction(context);
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                await action();
                transaction.Commit();
            }
        });
    }
}