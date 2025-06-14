using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Models;

namespace RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Services;
public interface IUserAuthorizationProvider
{
    Task<UserAuthorizationData?> GetUserAuthorizationAsync(Guid userId);
}
