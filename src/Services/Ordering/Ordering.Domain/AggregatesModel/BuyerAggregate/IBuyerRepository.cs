﻿namespace eShopWithoutContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
public interface IBuyerRepository : IRepository<Buyer>
{
    Buyer Add(Buyer buyer);
    Buyer Update(Buyer buyer);
    Task<Buyer> FindAsync(string buyerIdentityGuid);
    Task<Buyer> FindByIdAsync(string id);
}