using System.Security.Claims;

namespace RuanFa.FashionShop.Infrastructure.Security.Authentication.Context;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal? principal)
    {
        if (principal == null)
            return null;

        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }

    public static string? GetUsername(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.Name); 
    }
}
