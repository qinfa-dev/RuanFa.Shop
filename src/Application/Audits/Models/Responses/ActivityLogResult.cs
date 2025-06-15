namespace RuanFa.FashionShop.Application.Audits.Models.Responses;
public record ActivityLogResult
{
    public Guid Id { get; set; }
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
