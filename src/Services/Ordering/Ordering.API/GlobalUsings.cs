﻿global using Microsoft.AspNetCore;
global using Azure.Core;
global using Azure.Identity;
global using eShopWithoutContainers.Services.Ordering.API;
global using eShopWithoutContainers.Services.Ordering.Infrastructure;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Serilog;
global using System.Net;
global using Microsoft.Extensions.Options;
global using Polly;
global using Polly.Retry;
global using eShopWithoutContainers.Services.Ordering.API.Extensions;
global using eShopWithoutContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
global using eShopWithoutContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
global using eShopWithoutContainers.Services.Ordering.Domain.SeedWork;
global using System.Data.SqlClient;
global using Microsoft.EntityFrameworkCore;
global using eShopWithoutContainers.Services.Ordering.API.Infrastructure;
global using eShopWithoutContainers.BuildingBlocks.IntegrationEventLogEF;
global using Autofac.Extensions.DependencyInjection;
global using Autofac;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.EntityFrameworkCore;