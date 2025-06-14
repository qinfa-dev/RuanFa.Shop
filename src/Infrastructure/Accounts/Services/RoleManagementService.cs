using ErrorOr;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Claims;
using RuanFa.FashionShop.Application.Accounts.Models.Datas;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.Infrastructure.Accounts.Entities;
using RuanFa.FashionShop.Infrastructure.Accounts.Errors;
using RuanFa.FashionShop.Infrastructure.Accounts.Extensions;
using Serilog;

namespace RuanFa.FashionShop.Infrastructure.Accounts.Services;

internal class RoleManagementService : IRoleManagementService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleManagementService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        // Assign: Role manager instance
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        // Assign: User manager instance
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<ErrorOr<Updated>> AssignPermissionsToRoleAsync(Guid roleId, List<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve role by ID
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return InfrastructureErrors.Role.NotFound;

            // Validate: Permissions list is not null
            if (permissions == null)
                return InfrastructureErrors.Role.InvalidPermissions;

            // Check: Retrieve existing claims
            var existingClaims = await _roleManager.GetClaimsAsync(role);
            // Filter: Permission claims
            var permissionClaims = existingClaims
                .Where(c => c.Type == CustomClaims.Permission)
                .ToList();

            // Remove: Existing permission claims
            foreach (var claim in permissionClaims)
            {
                // Await: Remove claim
                var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
                if (!removeResult.Succeeded)
                    return removeResult.Errors.ToApplicationResult("RemovePermissionFailed");
            }

            // Add: New permission claims
            foreach (var permission in permissions.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                // Create: Permission claim
                var claim = new System.Security.Claims.Claim(CustomClaims.Permission, permission);
                // Await: Add claim to role
                var addResult = await _roleManager.AddClaimAsync(role, claim);
                if (!addResult.Succeeded)
                    return addResult.Errors.ToApplicationResult("AddPermissionFailed");
            }

            // Log: Permissions assigned
            Log.Information("Permissions assigned to role {RoleId}", roleId);
            return Result.Updated;
        }
        catch (Exception ex)
        {
            // Log: Permission assignment error
            Log.Error(ex, "Failed to assign permissions to role {RoleId}", roleId);
            // Escalate: Internal error
            return InfrastructureErrors.Role.AssignPermissionsInternal;
        }
    }

    public async Task<ErrorOr<Updated>> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return InfrastructureErrors.Account.NotFound;

            // Validate: Role IDs list is not null
            if (roleIds == null || !roleIds.Any())
                return InfrastructureErrors.Role.InvalidRoles;

            // Check: Retrieve existing roles
            var existingRoles = await _userManager.GetRolesAsync(user);
            // Remove: Current roles
            if (existingRoles.Any())
            {
                // Await: Remove all roles
                var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                if (!removeResult.Succeeded)
                    return removeResult.Errors.ToApplicationResult("RemoveRolesFailed");
            }

            // Filter: Valid role names
            var validRoleNames = new List<string>();
            foreach (var roleId in roleIds)
            {
                // Check: Retrieve role
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role != null && !string.IsNullOrEmpty(role.Name))
                    validRoleNames.Add(role.Name);
            }

            // Verify: At least one valid role
            if (!validRoleNames.Any())
                return InfrastructureErrors.Role.NotFound;

            // Await: Assign new roles
            var addResult = await _userManager.AddToRolesAsync(user, validRoleNames);
            if (!addResult.Succeeded)
                return addResult.Errors.ToApplicationResult("AssignRolesFailed");

            // Log: Roles assigned
            Log.Information("Roles assigned to user {UserId}", userId);
            return Result.Updated;
        }
        catch (Exception ex)
        {
            // Log: Role assignment error
            Log.Error(ex, "Failed to assign roles to user {UserId}", userId);
            // Escalate: Internal error
            return InfrastructureErrors.Role.AssignRolesInternal;
        }
    }

    public async Task<ErrorOr<RoleResult>> CreateRoleAsync(RoleInfo role, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate: Role info is not null
            if (role == null || string.IsNullOrWhiteSpace(role.Name))
                return InfrastructureErrors.Role.InvalidRoleName;

            // Check: Role does not exist
            if (await _roleManager.RoleExistsAsync(role.Name))
                return InfrastructureErrors.Role.AlreadyExists;

            // Create: New role
            var applicationRole = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = role.Name,
                NormalizedName = role.Name.ToUpperInvariant()
            };

            // Await: Create role
            var createResult = await _roleManager.CreateAsync(applicationRole);
            if (!createResult.Succeeded)
                return createResult.Errors.ToApplicationResult("CreateRoleFailed");

            // Add: Permissions as claims if provided
            if (role.Permissions != null && role.Permissions.Any())
            {
                foreach (var permission in role.Permissions.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    // Create: Permission claim
                    var claim = new System.Security.Claims.Claim(CustomClaims.Permission, permission);
                    // Await: Add claim to role
                    var addResult = await _roleManager.AddClaimAsync(applicationRole, claim);
                    if (!addResult.Succeeded)
                    {
                        // Compensate: Delete created role
                        await _roleManager.DeleteAsync(applicationRole);
                        // Escalate: Permission assignment failure
                        return addResult.Errors.ToApplicationResult("AddPermissionFailed");
                    }
                }
            }

            // Log: Role created
            Log.Information("Role {RoleName} created with ID {RoleId}", role.Name, applicationRole.Id);
            return applicationRole.Adapt<RoleResult>();
        }
        catch (Exception ex)
        {
            // Log: Role creation error
            Log.Error(ex, "Failed to create role {RoleName}", role?.Name);
            // Escalate: Internal error
            return InfrastructureErrors.Role.CreateInternal;
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve role
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return InfrastructureErrors.Role.NotFound;

            // Remove: All users from this role before deleting
            var usersInRole = await _userManager.Users
                .Where(u => _userManager.IsInRoleAsync(u, role.Name!).Result)
                .ToListAsync(cancellationToken);

            foreach (var user in usersInRole)
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, role.Name!);
                if (!removeResult.Succeeded)
                    return removeResult.Errors.ToApplicationResult("RemoveUserFromRoleFailed");
            }

            // Await: Delete role
            var deleteResult = await _roleManager.DeleteAsync(role);
            if (!deleteResult.Succeeded)
                return deleteResult.Errors.ToApplicationResult("DeleteRoleFailed");

            // Log: Role deleted
            Log.Information("Role {RoleId} deleted", roleId);
            return Result.Deleted;
        }
        catch (Exception ex)
        {
            // Log: Role deletion error
            Log.Error(ex, "Failed to delete role {RoleId}", roleId);
            // Escalate: Internal error
            return InfrastructureErrors.Role.DeleteInternal;
        }
    }

    public async Task<ErrorOr<List<RoleResult>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve all roles
            var roles = await _roleManager.Roles.Select(role => new RoleResult
            {
                Id = role.Id,
                Name = role.Name,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                LastModified = role.LastModified,
                LastModifiedBy = role.LastModifiedBy
            }).ToListAsync(cancellationToken);

            // Transform: Map to RoleResult
            var roleResults = roles.ToList();

            // Log: Roles retrieved
            Log.Information("Retrieved {RoleCount} roles", roleResults.Count);
            return roleResults;
        }
        catch (Exception ex)
        {
            // Log: Role retrieval error
            Log.Error(ex, "Failed to retrieve roles");
            // Escalate: Internal error
            return InfrastructureErrors.Role.RetrievalInternal;
        }
    }

    public async Task<ErrorOr<RoleDetailResult>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve role
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null || string.IsNullOrEmpty(role.Name))
                return InfrastructureErrors.Role.NotFound;

            // Check: Retrieve claims
            var claims = await _roleManager.GetClaimsAsync(role);
            // Filter: Permission claims
            var permissions = claims
                .Where(c => c.Type == CustomClaims.Permission)
                .Select(c => c.Value)
                .ToList();

            // Create: Role detail result
            var roleDetail = new RoleDetailResult
            {
                Id = role.Id,
                Name = role.Name,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                LastModified = role.LastModified,
                LastModifiedBy = role.LastModifiedBy,
                Permissions = permissions
            };

            // Log: Role retrieved
            Log.Information("Retrieved role {RoleId}", roleId);
            return roleDetail;
        }
        catch (Exception ex)
        {
            // Log: Role retrieval error
            Log.Error(ex, "Failed to retrieve role {RoleId}", roleId);
            // Escalate: Internal error
            return InfrastructureErrors.Role.RetrievalInternal;
        }
    }

    public async Task<ErrorOr<List<RoleResult>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return InfrastructureErrors.Account.NotFound;

            // Check: Retrieve user roles
            var roleNames = await _userManager.GetRolesAsync(user);

            // Transform: Map role names to RoleResult
            var roleResults = new List<RoleResult>();
            foreach (var roleName in roleNames)
            {
                // Check: Retrieve role
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roleResults.Add(new RoleResult
                    {
                        Id = role.Id,
                        Name = role.Name
                    });
                }
            }

            // Log: User roles retrieved
            Log.Information("Retrieved roles for user {UserId}", userId);
            return roleResults;
        }
        catch (Exception ex)
        {
            // Log: User roles retrieval error
            Log.Error(ex, "Failed to retrieve roles for user {UserId}", userId);
            // Escalate: Internal error
            return InfrastructureErrors.Role.RetrievalInternal;
        }
    }

    public async Task<ErrorOr<Updated>> UpdateRoleAsync(Guid roleId, RoleInfo updatedRole, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve role
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return InfrastructureErrors.Role.NotFound;

            // Validate: Updated role info is valid
            if (updatedRole == null || string.IsNullOrWhiteSpace(updatedRole.Name))
                return InfrastructureErrors.Role.InvalidRoleName;

            // Check: New name is not taken
            if (role.Name != updatedRole.Name && await _roleManager.RoleExistsAsync(updatedRole.Name))
                return InfrastructureErrors.Role.AlreadyExists;

            // Update: Role name
            role.Name = updatedRole.Name;
            role.NormalizedName = updatedRole.Name.ToUpperInvariant();

            // Await: Update role
            var updateResult = await _roleManager.UpdateAsync(role);
            if (!updateResult.Succeeded)
                return updateResult.Errors.ToApplicationResult("UpdateRoleFailed");

            // Update: Permissions if provided
            if (updatedRole.Permissions != null)
            {
                // Check: Retrieve existing claims
                var existingClaims = await _roleManager.GetClaimsAsync(role);
                // Filter: Permission claims
                var permissionClaims = existingClaims
                    .Where(c => c.Type == CustomClaims.Permission)
                    .ToList();

                // Remove: Existing permission claims
                foreach (var claim in permissionClaims)
                {
                    // Await: Remove claim
                    var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
                    if (!removeResult.Succeeded)
                        return removeResult.Errors.ToApplicationResult("RemovePermissionFailed");
                }

                // Add: New permission claims
                foreach (var permission in updatedRole.Permissions.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    // Create: Permission claim
                    var claim = new System.Security.Claims.Claim(CustomClaims.Permission, permission);
                    // Await: Add claim to role
                    var addResult = await _roleManager.AddClaimAsync(role, claim);
                    if (!addResult.Succeeded)
                        return addResult.Errors.ToApplicationResult("AddPermissionFailed");
                }
            }

            // Log: Role updated
            Log.Information("Role {RoleId} updated to {RoleName}", roleId, updatedRole.Name);
            return Result.Updated;
        }
        catch (Exception ex)
        {
            // Log: Role update error
            Log.Error(ex, "Failed to update role {RoleId}", roleId);
            // Escalate: Internal error
            return InfrastructureErrors.Role.UpdateInternal;
        }
    }

    public async Task<ErrorOr<bool>> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return InfrastructureErrors.Account.NotFound;

            // Check: Retrieve role
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null || string.IsNullOrEmpty(role.Name))
                return InfrastructureErrors.Role.NotFound;

            // Verify: User role membership
            var hasRole = await _userManager.IsInRoleAsync(user, role.Name);

            // Log: Role check result
            Log.Information("User {UserId} has role {RoleId}: {HasRole}", userId, roleId, hasRole);
            return hasRole;
        }
        catch (Exception ex)
        {
            // Log: Role check error
            Log.Error(ex, "Failed to check role {RoleId} for user {UserId}", roleId, userId);
            // Escalate: Internal error
            return InfrastructureErrors.Role.CheckRoleInternal;
        }
    }
}
