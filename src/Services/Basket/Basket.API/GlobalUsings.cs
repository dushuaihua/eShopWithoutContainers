﻿global using Azure.Core;
global using Azure.Identity;
global using eShopWithoutContainers.Services.Basket.API;
global using eShopWithoutContainers.Services.Basket.API.Infrastructure.Middlewares;
global using eShopWithoutContainers.Services.Basket.API.Infrastructure.Exceptions;
global using eShopWithoutContainers.Services.Basket.API.Infrastructure.Filters;
global using eShopWithoutContainers.Services.Basket.API.Infrastructure.ActionResults;
global using eShopWithoutContainers.Services.Basket.API.Controllers;
global using System.Net;
global using Microsoft.AspNetCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.AspNetCore.Authorization;
global using Serilog;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using System.IdentityModel.Tokens.Jwt;
global using StackExchange.Redis;
global using eShopWithoutContainers.BuildingBlocks.EventBus.Events;
global using eShopWithoutContainers.BuildingBlocks.EventBus.Abstractions;
global using eShopWithoutContainers.Services.Basket.API.IntegrationEvents.Events;
global using eShopWithoutContainers.Services.Basket.API.Model;
global using eShopWithoutContainers.BuildingBlocks.EventBus;
global using eShopWithoutContainers.BuildingBlocks.EventBusRabbitMQ;
global using eShopWithoutContainers.BuildingBlocks.EventBusServiceBus;
global using eShopWithoutContainers.Services.Basket.API.IntegrationEvents.EventHandling;
global using eShopWithoutContainers.Services.Basket.API.Infrastructure.Repositories;
global using eShopWithoutContainers.Services.Basket.API.Services;
global using Microsoft.Extensions.Options;
global using RabbitMQ.Client;
global using System.ComponentModel.DataAnnotations;
global using Serilog.Context;
global using Autofac;
global using Autofac.Extensions.DependencyInjection;