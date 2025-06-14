using Microsoft.AspNetCore.Authorization;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Policies;

namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class ApiAuthorizeAttribute : AuthorizeAttribute
{
    public ApiAuthorizeAttribute(string? permissions = null)
    {
        Permissions = permissions;
        Policy = BuildPolicy();
    }

    public ApiAuthorizeAttribute(
        string? permissions = null,
        string? policies = null,
        string? roles = null)
    {
        Permissions = permissions;
        Policies = policies;
        Roles = roles;
        Policy = BuildPolicy();
    }

    public string? Permissions { get; set; }
    public string? Policies { get; set; }

    private string BuildPolicy()
    {
        var policyParts = new List<string>();

        if (!string.IsNullOrEmpty(Permissions))
            policyParts.Add($"{PolicyPrefix.Permission}{Permissions}");

        if (!string.IsNullOrEmpty(Policies))
            policyParts.Add($"{PolicyPrefix.Policy}{Policies}");

        if (!string.IsNullOrEmpty(Roles))
            policyParts.Add($"{PolicyPrefix.Role}{Roles}");

        return string.Join(";", policyParts);
    }
}
