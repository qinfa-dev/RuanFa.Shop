using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Contexts;

namespace RuanFa.FashionShop.Infrastructure.Security.Authentication.Context;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId => _httpContextAccessor.HttpContext?.User.GetUserId();

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public string? Username => _httpContextAccessor.HttpContext?.User.GetUsername();
}
