using System.Text.Json;
using Microsoft.AspNetCore.Http;
using RuanFa.FashionShop.SharedKernel.Domains.Entities;

namespace RuanFa.FashionShop.Application.Abstractions.Loggings.Entities;
public class ActivityLogEntry : Entity<Guid>
{
    public string? Username { get; set; }
    public required string Activity { get; set; }
    public required string Endpoint { get; set; }
    public required string HttpMethod { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public required string IpAddress { get; set; }
    public required string UserAgent { get; set; }
    public string? Result { get; set; }
    public string? Request { get; set; }
}
public static class ActivityLogEntryExtensions
{
    public static ActivityLogEntry CreateFromRequest(
        this ActivityLogEntry _,
        string username,
        string activity,
        HttpRequest httpRequest,
        string? result = null,
        object? metadata = null)
    {
        return new ActivityLogEntry
        {
            Username = username,
            Activity = activity,
            Endpoint = httpRequest.Path,
            HttpMethod = httpRequest.Method,
            Timestamp = DateTimeOffset.UtcNow,
            IpAddress = httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            UserAgent = httpRequest.Headers["User-Agent"].ToString(),
            Result = result,
            Request = metadata == null ? null : JsonSerializer.Serialize(metadata)
        };
    }
}
