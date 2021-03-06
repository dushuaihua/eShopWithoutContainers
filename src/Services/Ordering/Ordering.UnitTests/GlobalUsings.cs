global using eShopWithoutContainers.Services.Ordering.API.Application.Commands;
global using eShopWithoutContainers.Services.Ordering.API.Application.IntegrationEvents;
global using eShopWithoutContainers.Services.Ordering.API.Application.Models;
global using eShopWithoutContainers.Services.Ordering.API.Application.Queries;
global using eShopWithoutContainers.Services.Ordering.API.Controllers;
global using eShopWithoutContainers.Services.Ordering.API.Infrastructure.Services;
global using eShopWithoutContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
global using eShopWithoutContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
global using eShopWithoutContainers.Services.Ordering.Domain.Events;
global using eShopWithoutContainers.Services.Ordering.Domain.Exceptions;
global using eShopWithoutContainers.Services.Ordering.Domain.SeedWork;
global using eShopWithoutContainers.Services.Ordering.Infrastructure.Idempotency;
global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Logging;
global using Moq;
global using System.Net;
global using Xunit;
global using CardType = eShopWithoutContainers.Services.Ordering.API.Application.Queries.CardType;
global using Order = eShopWithoutContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate.Order;
