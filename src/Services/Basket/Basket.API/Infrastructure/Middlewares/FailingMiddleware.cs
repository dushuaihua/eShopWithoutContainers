﻿namespace eShopWithoutContainers.Services.Basket.API.Infrastructure.Middlewares;

using Microsoft.Extensions.Logging;
public class FailingMiddleware
{
    private readonly RequestDelegate _next;
    private bool _mustFail;
    private readonly FailingOptions _options;
    private readonly ILogger _logger;

    public FailingMiddleware(RequestDelegate next, ILogger<FailingMiddleware> logger, FailingOptions options)
    {
        _next = next;
        _options = options;
        _mustFail = false;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path;
        if (path.Equals(_options.ConfigPath, StringComparison.OrdinalIgnoreCase))
        {
            await ProcessConfigRequest(context);
            return;
        }

        if (MustFail(context))
        {
            _logger.LogInformation("Response for path {Path} will fail.", path);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Failed due to FaillingMiddleware enabled.");
        }
        else
        {
            await _next.Invoke(context);
        }
    }
    private async Task ProcessConfigRequest(HttpContext context)
    {
        var enable = context.Request.Query.Keys.Any(k => k == "enable");
        var disable = context.Request.Query.Keys.Any(k => k == "disable");

        if (enable && disable)
        {
            throw new ArgumentException("Must use enable or disable querystring values, but not both");
        }
        if (disable)
        {
            _mustFail = false;
            await SendOkResponse(context, "FailingMiddleware disabled. Future requests will be processed.");
            return;
        }
        if (enable)
        {
            _mustFail = true;
            await SendOkResponse(context, "FailingMiddleware enabled. Futher requests will return HTTP 500");
            return;
        }

        await SendOkResponse(context, string.Format("FailingMiddleware is {0}", _mustFail ? "enabled" : "disabled"));
        return;
    }

    private async Task SendOkResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync(message);
    }

    private bool MustFail(HttpContext context)
    {
        var rpath = context.Request.Path.Value;

        if (_options.NotFilteredPaths.Any(p => p.Equals(rpath, StringComparison.InvariantCultureIgnoreCase)))
        {
            return false;
        }

        return _mustFail &&
            (_options.EndpointPaths.Any(x => x == rpath)
            || _options.EndpointPaths.Count == 0);
    }
}
