using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Abstractions.Loggings.Attributes;
using RuanFa.FashionShop.Application.Abstractions.Loggings.Entities;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Contexts;
using RuanFa.FashionShop.SharedKernel.Interfaces.System;
using Serilog;
using Serilog.Context;

namespace RuanFa.FashionShop.Application.Abstractions.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : IErrorOr
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IUserContext _userContext = userContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        // Always log the start of request processing
        Log.Information("Processing request {RequestName}", requestName);

        TResponse result = await next();

        // Always log completion (success or error)
        if (!result.IsError)
        {
            Log.Information("Completed request {RequestName}", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Error", result.Errors, true))
            {
                Log.Error("Completed request {RequestName} with error", requestName);
            }
        }

        // Now check if activity log attribute is present, and if yes, save activity log to DB
        var logAttr = typeof(TRequest).GetCustomAttribute<LogActivityAttribute>();
        if (logAttr != null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && _userContext.IsAuthenticated && _userContext.UserId != null)
            {
                requestName = requestName = Regex.Replace(
                    input: Regex.Replace(typeof(TRequest).Name, "(Command|Query|Request)$", ""),
                    @"([a-z0-9])([A-Z])",
                    "$1_$2").ToLower();
                var activityLog = new ActivityLogEntry
                {
                    Username = _userContext.Username,
                    Activity = logAttr.ActionName ?? requestName,
                    Endpoint = httpContext.Request.Path,
                    HttpMethod = httpContext.Request.Method,
                    Timestamp = DateTimeOffset.UtcNow,
                    IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                    Request = System.Text.Json.JsonSerializer.Serialize(request),
                    Result = result.IsError ? JsonSerializer.Serialize(result.Errors) : "Success",
                };

                _dbContext.ActivityLogs.Add(activityLog);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        return result;
    }
}
