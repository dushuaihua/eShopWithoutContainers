namespace eShopWithoutContainers.Services.Basket.API.Infrastructure.Repositories;
public class RedisBasketRepository : IBasketRepository
{
    public Task<bool> DeleteBasketAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<CustomerBasket> GetBasketAsync(string customerId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetUsers()
    {
        throw new NotImplementedException();
    }

    public Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        throw new NotImplementedException();
    }
}