using Microsoft.AspNetCore.Authorization;

namespace RuanFa.FashionShop.Infrastructure.Security.Authorization.Requirements;

internal class HasAuthorizationRequirement(
    string[] permissions,
    string[] policies,
    string[] roles) : IAuthorizationRequirement
{
    public string[] Permissions { get; } = permissions ?? Array.Empty<string>();
    public string[] Policies { get; } = policies ?? Array.Empty<string>();
    public string[] Roles { get; } = roles ?? Array.Empty<string>();
}
