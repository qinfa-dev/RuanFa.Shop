using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Claims;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Permissions;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Roles;
using RuanFa.FashionShop.Domain.Accounts.Entities;
using RuanFa.FashionShop.Domain.Accounts.ValueObjects;
using RuanFa.FashionShop.Domain.Commons.Enums;
using RuanFa.FashionShop.Domain.Commons.ValueObjects;
using RuanFa.FashionShop.Infrastructure.Accounts.Entities;
using Serilog;
using System.Security.Claims;

namespace RuanFa.FashionShop.Infrastructure.Data.Seeds;

public class IdentitySeedProvider : IDataSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly List<(string addressLine1, string addressLine2, string city, string state,
        string country, string postalCode, string boutiquePickupLocation, string deliveryInstructions,
        AddressType type, bool isDefault)> DefaultAddresses = new()
    {
        ("1 Apple Park Way", "", "Cupertino", "CA", "USA", "95014",
            "Apple Park Visitor Center", "Leave at the front desk", AddressType.Shipping, true),
        ("1 Apple Park Way", "", "Cupertino", "CA", "USA", "95014",
            "Apple Park Visitor Center", "Leave at the front desk", AddressType.Billing, false)
    };

    private static readonly List<string> DefaultWishlist = new()
    {
        "Classic T-Shirt", "Blue Jeans", "Leather Jacket"
    };

    public IdentitySeedProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Log.Information("[IdentitySeed] Starting role and user seeding");

        try
        {
            await SeedRolesAsync(roleManager, cancellationToken);
            await SeedUsersAsync(userManager, dbContext, cancellationToken);
            Log.Information("[IdentitySeed] Seeding completed successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[IdentitySeed] Seeding failed");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, CancellationToken cancellationToken)
    {
        Log.Information("[IdentitySeed:Roles] Seeding roles");

        var rolePermissions = new Dictionary<string, List<string>>
        {
            { DefaultRoles.Administrator, Permission.AdministratorMoudle },
            { DefaultRoles.User, Permission.UserMoudle }
        };

        var tasks = rolePermissions.Select(async kvp =>
        {
            try
            {
                await ProcessRoleAsync(roleManager, kvp.Key, kvp.Value, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[IdentitySeed:Roles] Failed processing role {RoleName}", kvp.Key);
            }
        });

        await Task.WhenAll(tasks);
        Log.Information("[IdentitySeed:Roles] Role seeding completed");
    }

    private static async Task ProcessRoleAsync(RoleManager<ApplicationRole> roleManager, string roleName,
        List<string> permissions, CancellationToken cancellationToken)
    {
        var appRole = await roleManager.FindByNameAsync(roleName);

        if (appRole == null)
        {
            appRole = new ApplicationRole(roleName);
            var result = await roleManager.CreateAsync(appRole);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                Log.Error("[IdentitySeed:Roles] Failed creating role {RoleName}: {Errors}", roleName, errors);
                return;
            }
            Log.Information("[IdentitySeed:Roles] Created role {RoleName}", roleName);
        }

        await SyncRolePermissionsAsync(roleManager, appRole, permissions, cancellationToken);
    }

    private static async Task SyncRolePermissionsAsync(RoleManager<ApplicationRole> roleManager,
        ApplicationRole role, List<string> targetPermissions, CancellationToken cancellationToken)
    {
        var existingClaims = await roleManager.GetClaimsAsync(role);
        var currentPermissions = existingClaims
            .Where(c => c.Type == CustomClaims.Permission)
            .Select(c => c.Value)
            .ToHashSet();

        var toAdd = targetPermissions.Except(currentPermissions);
        var toRemove = currentPermissions.Except(targetPermissions);

        // Add new permissions
        var addTasks = toAdd.Select(async permission =>
        {
            var result = await roleManager.AddClaimAsync(role, new Claim(CustomClaims.Permission, permission));
            if (result.Succeeded)
                Log.Information("[IdentitySeed:Roles] Added permission {Permission} to {RoleName}", permission, role.Name);
            else
                Log.Error("[IdentitySeed:Roles] Failed adding permission {Permission} to {RoleName}", permission, role.Name);
        });

        // Remove outdated permissions
        var removeTasks = toRemove.Select(async permission =>
        {
            var claim = existingClaims.FirstOrDefault(c => c.Value == permission);
            if (claim != null)
            {
                var result = await roleManager.RemoveClaimAsync(role, claim);
                if (result.Succeeded)
                    Log.Information("[IdentitySeed:Roles] Removed permission {Permission} from {RoleName}", permission, role.Name);
                else
                    Log.Error("[IdentitySeed:Roles] Failed removing permission {Permission} from {RoleName}", permission, role.Name);
            }
        });

        await Task.WhenAll(addTasks.Concat(removeTasks));
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        Log.Information("[IdentitySeed:Users] Seeding users");

        var users = new[]
        {
            ("admin1@quingfa.com", "Admin1Password!", "Admin One", "1234567890", GenderType.Male, new DateTime(1990, 1, 1), DefaultRoles.Administrator),
            ("admin2@quingfa.com", "Admin2Password!", "Admin Two", "1234567891", GenderType.Male, new DateTime(1990, 2, 2), DefaultRoles.Administrator),
            ("user1@quingfa.com", "User1Password!", "User One", "0987654321", GenderType.Female, new DateTime(1995, 5, 5), DefaultRoles.User),
            ("user2@quingfa.com", "User2Password!", "User Two", "0987654322", GenderType.Female, new DateTime(1995, 6, 6), DefaultRoles.User)
        };

        var tasks = users.Select(async user =>
        {
            try
            {
                await ProcessUserAsync(userManager, dbContext, user, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[IdentitySeed:Users] Failed processing user {Email}", user.Item1);
            }
        });

        await Task.WhenAll(tasks);
        Log.Information("[IdentitySeed:Users] User seeding completed");
    }

    private static async Task ProcessUserAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext,
        (string Email, string Password, string FullName, string Phone, GenderType Gender, DateTime BirthDateUtc, string Role) userInfo,
        CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(userInfo.Email);

        if (existingUser != null)
        {
            await UpdateExistingUserAsync(userManager, dbContext, existingUser, userInfo, cancellationToken);
            return;
        }

        await CreateNewUserAsync(userManager, dbContext, userInfo, cancellationToken);
    }

    private static async Task CreateNewUserAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext,
        (string Email, string Password, string FullName, string Phone, GenderType Gender, DateTime BirthDateUtc, string Role) userInfo,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = userInfo.Email,
            Email = userInfo.Email,
            PhoneNumber = userInfo.Phone,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "IdentitySeed",
            LastModified = DateTime.UtcNow,
            LastModifiedBy = "IdentitySeed"
        };

        Log.Information("[IdentitySeed:Users] Creating user {Email}", userInfo.Email);

        //await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Create user
            var createResult = await userManager.CreateAsync(user, userInfo.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                Log.Error("[IdentitySeed:Users] Failed creating user {Email}: {Errors}", userInfo.Email, errors);
                return;
            }

            // Create profile
            var (profile, profileSuccess) = await CreateUserProfileAsync(dbContext, user.Id, userInfo, cancellationToken);
            if (!profileSuccess)
            {
                Log.Error("[IdentitySeed:Users] Failed creating profile for {Email}", userInfo.Email);
                return;
            }

            // Assign role
            var roleResult = await userManager.AddToRoleAsync(user, userInfo.Role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                Log.Error("[IdentitySeed:Users] Failed assigning role {Role} to {Email}: {Errors}",
                    userInfo.Role, userInfo.Email, errors);
                return;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            //await transaction.CommitAsync(cancellationToken);

            Log.Information("[IdentitySeed:Users] Successfully created user {Email} with role {Role}",
                userInfo.Email, userInfo.Role);
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(cancellationToken);
            Log.Error(ex, "[IdentitySeed:Users] Exception creating user {Email}", userInfo.Email);
            throw;
        }
    }

    private static async Task UpdateExistingUserAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext,
        ApplicationUser user, (string Email, string Password, string FullName, string Phone, GenderType Gender, DateTime BirthDateUtc, string Role) userInfo,
        CancellationToken cancellationToken)
    {
        Log.Information("[IdentitySeed:Users] Updating existing user {Email}", user.Email);

        var updateTasks = new List<Task>();

        // Update phone if different
        if (user.PhoneNumber != userInfo.Phone)
        {
            user.PhoneNumber = userInfo.Phone;
            updateTasks.Add(userManager.UpdateAsync(user));
        }

        // Ensure user has required role
        var userRoles = await userManager.GetRolesAsync(user);
        if (!userRoles.Contains(userInfo.Role))
        {
            updateTasks.Add(AssignRoleIfMissingAsync(userManager, user, userInfo.Role));
        }

        // Ensure user has profile
        updateTasks.Add(EnsureUserProfileExistsAsync(dbContext, user, userInfo, cancellationToken));

        await Task.WhenAll(updateTasks);
    }

    private static async Task AssignRoleIfMissingAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, string role)
    {
        var roleResult = await userManager.AddToRoleAsync(user, role);
        if (roleResult.Succeeded)
            Log.Information("[IdentitySeed:Users] Added role {Role} to existing user {Email}", role, user.Email);
        else
            Log.Error("[IdentitySeed:Users] Failed adding role {Role} to existing user {Email}", role, user.Email);
    }

    private static async Task EnsureUserProfileExistsAsync(ApplicationDbContext dbContext, ApplicationUser user,
        (string Email, string Password, string FullName, string Phone, GenderType Gender, DateTime BirthDateUtc, string Role) userInfo,
        CancellationToken cancellationToken)
    {
        var existingProfile = await dbContext.Profiles
            .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);

        if (existingProfile == null)
        {
            Log.Information("[IdentitySeed:Users] Creating missing profile for {Email}", user.Email);
            await CreateUserProfileAsync(dbContext, user.Id, userInfo, cancellationToken);
        }
        else if (existingProfile.FullName != userInfo.FullName)
        {
            await UpdateUserProfileAsync(dbContext, existingProfile, userInfo.FullName, cancellationToken);
        }
    }

    private static async Task UpdateUserProfileAsync(ApplicationDbContext dbContext, UserProfile profile,
        string newFullName, CancellationToken cancellationToken)
    {
        var updateResult = profile.UpdatePersonalDetails(
            email: profile.Email,
            username: profile.Username,
            fullName: newFullName,
            phoneNumber: profile.PhoneNumber,
            gender: profile.Gender,
            dateOfBirth: profile.DateOfBirth,
            marketingConsent: profile.MarketingConsent);

        if (!updateResult.IsError)
        {
            dbContext.Profiles.Update(profile);
            await dbContext.SaveChangesAsync(cancellationToken);
            Log.Information("[IdentitySeed:Users] Updated profile for {Email}", profile.Email);
        }
        else
        {
            Log.Error("[IdentitySeed:Users] Failed updating profile for {Email}: {Error}",
                profile.Email, updateResult.FirstError);
        }
    }

    private static async Task<(UserProfile? profile, bool success)> CreateUserProfileAsync(ApplicationDbContext dbContext,
        Guid applicationUserId, (string Email, string Password, string FullName, string Phone, GenderType Gender, DateTime BirthDateUtc, string Role) userInfo,
        CancellationToken cancellationToken)
    {
        // Create addresses
        var addresses = DefaultAddresses
            .Select(addr => UserAddress.Create(
                addr.addressLine1, addr.addressLine2, addr.city, addr.state,
                addr.country, addr.postalCode, addr.boutiquePickupLocation,
                addr.deliveryInstructions, addr.type, addr.isDefault))
            .Where(addr => addr != null)
            .ToList()!;

        // Create user profile
        var profileResult = UserProfile.Create(
            userId: applicationUserId,
            username: userInfo.Email.Split('@')[0],
            email: userInfo.Email,
            fullName: userInfo.FullName,
            phoneNumber: userInfo.Phone,
            gender: userInfo.Gender,
            dateOfBirth: userInfo.BirthDateUtc != default ? new DateTimeOffset(userInfo.BirthDateUtc, TimeSpan.Zero) : null,
            addresses: addresses,
            preferences: GetDefaultPreferences(),
            wishlist: DefaultWishlist,
            loyaltyPoints: 0,
            marketingConsent: true
        );

        if (profileResult.IsError)
        {
            Log.Error("[IdentitySeed:Users] Failed creating profile for {Email}: {Error}",
                userInfo.Email, profileResult.FirstError);
            return (null, false);
        }

        var profile = profileResult.Value;
        var now = DateTime.UtcNow;
        profile.CreatedAt = now;
        profile.CreatedBy = "IdentitySeed";
        profile.LastModified = now;
        profile.LastModifiedBy = "IdentitySeed";

        dbContext.Profiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);

        Log.Information("[IdentitySeed:Users] Created profile for {Email}", userInfo.Email);
        return (profile, true);
    }

    private static FashionPreference GetDefaultPreferences()
    {
        return FashionPreference.Create(
            clothingSize: "M",
            favoriteCategories: new List<string> { "Shirts", "Pants", "Accessories" }
        ) ?? new FashionPreference();
    }
}
