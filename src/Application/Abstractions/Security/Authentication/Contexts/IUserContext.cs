using System.Security.Claims;

namespace RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Contexts;
public interface IUserContext
{
    Guid? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
    ClaimsPrincipal? User { get; }
}
