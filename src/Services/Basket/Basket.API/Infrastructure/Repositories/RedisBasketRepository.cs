namespace eShopWithoutContainers.Services.Basket.API.Infrastructure.Repositories;
public class RedisBasketRepository : IBasketRepository
{
    private readonly ILogger<RedisBasketRepository> _logger;
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;

    public RedisBasketRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer connectionMultiplexer)
    {
        _logger = loggerFactory.CreateLogger<RedisBasketRepository>();
        _connectionMultiplexer = connectionMultiplexer;
        _database = _connectionMultiplexer.GetDatabase();
    }
    public async Task<bool> DeleteBasketAsync(string id)
    {
        return await _database.KeyDeleteAsync(id);
    }

    public async Task<CustomerBasket> GetBasketAsync(string customerId)
    {
        var data = await _database.StringGetAsync(customerId);

        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<CustomerBasket>(data, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public IEnumerable<string> GetUsers()
    {
        var server = GetServer();
        var data = server.Keys();

        return data?.Select(k => k.ToString());
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var created = await _database.StringSetAsync(basket.BuyerId, JsonSerializer.Serialize(basket));

        if (!created)
        {
            _logger.LogInformation("Problem occur persisting the item.");
            return null;
        }

        _logger.LogInformation("Basket item persisted successfully.");

        return await GetBasketAsync(basket.BuyerId);
    }

    private IServer GetServer()
    {
        var endpoint = _connectionMultiplexer.GetEndPoints();
        return _connectionMultiplexer.GetServer(endpoint.First());
    }
}