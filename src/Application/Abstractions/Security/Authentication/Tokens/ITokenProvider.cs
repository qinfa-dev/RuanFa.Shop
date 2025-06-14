using System.Security.Claims;

namespace RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Tokens;
public interface ITokenProvider
{
    string CreateAccessToken(string id, string username, string? email);
    string CreateRefreshToken(string? id);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
