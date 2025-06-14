using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Claims;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Models;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Services;
using RuanFa.FashionShop.Infrastructure.Accounts.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace RuanFa.FashionShop.Infrastructure.Security.Authorization.Providers;

public class UserAuthorizationProvider : IUserAuthorizationProvider
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<UserAuthorizationProvider> _logger;
    private readonly IDistributedCache _cache;

    public UserAuthorizationProvider(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UserAuthorizationProvider> logger,
        IDistributedCache cache)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _cache = cache;
    }

    public async Task<UserAuthorizationData?> GetUserAuthorizationAsync(Guid userId)
    {
        var cacheKey = $"UserAuthorization_{userId}";

        // Try to get from distributed cache
        var cachedUserJson = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedUserJson))
        {
            try
            {
                var cachedUser = JsonSerializer.Deserialize<UserAuthorizationData>(cachedUserJson);
                if (cachedUser != null)
                {
                    _logger.LogInformation("Distributed cache hit for user authorization: {UserId}", userId);
                    return cachedUser;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize cached user authorization for {UserId}", userId);
                // Remove corrupted cache entry
                await _cache.RemoveAsync(cacheKey);
            }
        }

        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            _logger.LogWarning("User authorization failed: User {UserId} not found.", userId);
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claimsForRole = await _roleManager.GetClaimsAsync(role);
                roleClaims.AddRange(claimsForRole);
            }
        }

        var permissions = roleClaims
            .Where(c => c.Type == CustomClaims.Permission)
            .Select(c => c.Value)
            .ToList()
            .AsReadOnly();

        var policies = roleClaims
            .Where(c => c.Type == CustomClaims.Policy)
            .Select(c => c.Value)
            .ToList()
            .AsReadOnly();

        var currentUser = new UserAuthorizationData(
            UserId: userId,
            Email: user.Email ?? string.Empty,
            Permissions: permissions,
            Roles: roles.ToList().AsReadOnly(),
            Policies: policies);

        // Cache with distributed cache
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2) // Optional: refresh cache if accessed within 2 minutes
        };

        try
        {
            var serializedUser = JsonSerializer.Serialize(currentUser);
            await _cache.SetStringAsync(cacheKey, serializedUser, cacheOptions);
            _logger.LogInformation("User authorization cached for {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache user authorization for {UserId}", userId);
            // Continue without caching - don't fail the request
        }

        _logger.LogInformation("User authorization retrieved for {UserId}", userId);
        return currentUser;
    }

    // Optional: Add a method to invalidate cache when user data changes
    public async Task InvalidateUserAuthorizationAsync(Guid userId)
    {
        var cacheKey = $"UserAuthorization_{userId}";
        await _cache.RemoveAsync(cacheKey);
        _logger.LogInformation("User authorization cache invalidated for {UserId}", userId);
    }
}
