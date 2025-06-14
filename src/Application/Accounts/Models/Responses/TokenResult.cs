namespace RuanFa.FashionShop.Application.Accounts.Models.Responses;
public record TokenResult
{
    public string AccessToken { get; init; } = string.Empty;
    public long ExpiresIn { get; init; }
    public string? RefreshToken { get; init; }
    public string TokenType { get; } = "Bearer";
}
