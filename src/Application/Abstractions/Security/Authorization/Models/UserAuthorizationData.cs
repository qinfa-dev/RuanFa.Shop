using System.Text.Json.Serialization;

namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Models;

public record UserAuthorizationData(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("permissions")] IReadOnlyList<string> Permissions,
    [property: JsonPropertyName("roles")] IReadOnlyList<string> Roles,
    [property: JsonPropertyName("policies")] IReadOnlyList<string> Policies
);
