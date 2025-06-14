namespace RuanFa.FashionShop.Application.Accounts.Models.Responses;
public record AccountInfoResult(
    Guid UserId,
    string Email,
    string? FullName,
    bool IsEmailVerified,
    DateTimeOffset? Created,
    DateTimeOffset? LastLogin,
    List<string>? Roles = null,
    List<string>? Permisions = null
);
