using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Policies;
using RuanFa.FashionShop.Infrastructure.Security.Authorization.Requirements;

namespace RuanFa.FashionShop.Infrastructure.Security.Authorization.Providers;

internal class HasAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    public HasAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return Task.FromResult(_options.DefaultPolicy);
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        // FallbackPolicy can be null
        return Task.FromResult(_options.FallbackPolicy);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
            throw new ArgumentException("Policy name cannot be null or empty.", nameof(policyName));

        var existingPolicy = _options.GetPolicy(policyName);
        if (existingPolicy != null)
        {
            return Task.FromResult<AuthorizationPolicy?>(existingPolicy);
        }

        var permissions = new List<string>();
        var policies = new List<string>();
        var roles = new List<string>();

        var policyParts = policyName.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in policyParts)
        {
            if (part.StartsWith(PolicyPrefix.Permission, StringComparison.OrdinalIgnoreCase))
            {
                var permission = part.Substring(PolicyPrefix.Permission.Length);
                permissions.AddRange(permission.Split(';', StringSplitOptions.RemoveEmptyEntries));
            }
            else if (part.StartsWith(PolicyPrefix.Policy, StringComparison.OrdinalIgnoreCase))
            {
                var policy = part.Substring(PolicyPrefix.Policy.Length);
                policies.AddRange(policy.Split(';', StringSplitOptions.RemoveEmptyEntries));
            }
            else if (part.StartsWith(PolicyPrefix.Role, StringComparison.OrdinalIgnoreCase))
            {
                var role = part.Substring(PolicyPrefix.Role.Length);
                roles.AddRange(role.Split(';', StringSplitOptions.RemoveEmptyEntries));
            }
        }

        if (!permissions.Any() && !policies.Any() && !roles.Any())
            return Task.FromResult<AuthorizationPolicy?>(null); // Invalid format

        var requirement = new HasAuthorizationRequirement(
            permissions.ToArray(),
            policies.ToArray(),
            roles.ToArray());

        var policyBuilder = new AuthorizationPolicyBuilder();
        policyBuilder.AddRequirements(requirement);

        return Task.FromResult<AuthorizationPolicy?>(policyBuilder.Build());
    }
}
