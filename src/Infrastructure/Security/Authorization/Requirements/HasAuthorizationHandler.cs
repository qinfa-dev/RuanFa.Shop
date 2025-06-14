using Microsoft.AspNetCore.Authorization;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Contexts;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Services;

namespace RuanFa.FashionShop.Infrastructure.Security.Authorization.Requirements;

internal class HasAuthorizationHandler(
    IUserContext userContext,
    IUserAuthorizationProvider authorizationProvider) : AuthorizationHandler<HasAuthorizationRequirement>
{
    private readonly IUserContext _userContext = userContext;
    private readonly IUserAuthorizationProvider _authorizationProvider = authorizationProvider;

    protected override async Task HandleRequirementAsync(
     AuthorizationHandlerContext context,
     HasAuthorizationRequirement requirement)
    {
        if (!_userContext.IsAuthenticated || _userContext.UserId == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "User not authenticated"));
            return;
        }

        var currentUser = await _authorizationProvider.GetUserAuthorizationAsync(_userContext.UserId.Value);
        if (currentUser == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "User data not found"));
            return;
        }

        foreach (var permission in requirement.Permissions)
        {
            if (!currentUser.Permissions.Contains(permission))
            {
                context.Fail(new AuthorizationFailureReason(this, $"Missing permission: {permission}"));
                return;
            }
        }

        foreach (var policy in requirement.Policies)
        {
            if (!currentUser.Policies.Contains(policy))
            {
                context.Fail(new AuthorizationFailureReason(this, $"Missing policy: {policy}"));
                return;
            }
        }

        foreach (var role in requirement.Roles)
        {
            if (!currentUser.Roles.Contains(role))
            {
                context.Fail(new AuthorizationFailureReason(this, $"Missing role: {role}"));
                return;
            }
        }

        context.Succeed(requirement);
    }

}
