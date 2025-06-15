using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Requests;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;

namespace RuanFa.FashionShop.Application.Accounts.Services;

/// <summary>
/// Provides operations for managing roles and user-role assignments.
/// </summary>
public interface IRoleManagementService
{
    /// <summary>
    /// Checks if a user has a specific role by role ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="roleId">The ID of the role.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the user has the role; otherwise, false. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<bool>> UserHasRoleAsync(Guid userId,  Guid roleId, CancellationToken cancellationToken = default);

    // ---------------------
    // Role Management
    // ---------------------

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="role">The role information including name and permissions.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The ID of the newly created role. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<RoleResult>> CreateRoleAsync(RoleInfo role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role by its ID.
    /// </summary>
    /// <param name="roleId">The ID of the role to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Success"/> indicator if the operation was successful. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<Deleted>> DeleteRoleAsync( Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing role using the provided role information.
    /// </summary>
    /// <param name="roleId">The ID of the role to update.</param>
    /// <param name="updatedRole">The new role information.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Success"/> indicator if the operation was successful. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<Updated>> UpdateRoleAsync( Guid roleId, RoleInfo updatedRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns or updates the list of permissions for a given role.
    /// </summary>
    /// <param name="roleId">The ID of the role.</param>
    /// <param name="permissions">A list of permission names to assign.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Success"/> indicator if the operation was successful. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<Updated>> AssignPermissionsToRoleAsync( Guid roleId, List<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all roles in the system.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of <see cref="RoleResult"/> objects. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<List<RoleResult>>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed information about a role by ID.
    /// </summary>
    /// <param name="roleId">The ID of the role.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="RoleDetailResult"/> object. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<RoleDetailResult>> GetRoleByIdAsync( Guid roleId, CancellationToken cancellationToken = default);

    // ---------------------
    // User Role Management
    // ---------------------

    /// <summary>
    /// Assigns multiple roles to a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="roleIds">A list of role IDs to assign.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Success"/> indicator if the operation was successful. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<Updated>> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a single role to a user.
    /// </summary>
    /// <param name="roleId">The ID of the role to assign.</param>
    /// <param name="userIds">>A list of user IDs to assign.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Success"/> indicator if the operation was successful. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<Updated>> AssignUsersToRoleAsync(Guid roleId, List<Guid> userIds,  CancellationToken cancellationToken = default);


    /// <summary>
    /// Retrieves all roles assigned to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of <see cref="RoleResult"/> objects. Wrapped in <see cref="ErrorOr{T}"/>.</returns>
    Task<ErrorOr<List<RoleResult>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
