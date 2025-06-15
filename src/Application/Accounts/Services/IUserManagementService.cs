using ErrorOr;
using RuanFa.FashionShop.Application.Accounts.Models.Requests;
using RuanFa.FashionShop.Domain.Accounts.Enums;


namespace RuanFa.FashionShop.Application.Accounts.Services;

/// <summary>
/// Provides user account management operations such as creating, retrieving, updating, deactivating, and deleting user profiles.
/// </summary>
public interface IUserManagementService
{
    /// <summary>
    /// Retrieves the user profile associated with the given user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing the <see cref="UserProfileInfo"/> if found, or an error.</returns>
    Task<ErrorOr<UserProfileInfo>> GetUserProfileByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the user profile associated with the specified email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing the <see cref="UserProfileInfo"/> if found, or an error.</returns>
    Task<ErrorOr<UserProfileInfo>> GetUserProfileByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user account with the provided profile and password information.
    /// </summary>
    /// <param name="accountInfo">The profile and password information for the new user.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> containing the created <see cref="UserProfileInfo"/> or an error.</returns>
    Task<ErrorOr<UserProfileInfo>> CreateUserAsync(
        RegisterAccountInfo accountInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the profile information of an existing user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to update.</param>
    /// <param name="updatedProfile">The updated user profile information.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating whether the update was successful or containing an error.</returns>
    Task<ErrorOr<Updated>> UpdateUserProfileAsync(
        string userId,
        UserProfileInfo updatedProfile,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the user account with the specified user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating whether the deletion was successful or containing an error.</returns>
    Task<ErrorOr<Deleted>> DeleteUserAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of the user account with the specified user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="newStatus">The new status to assign to the user account.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>An <see cref="ErrorOr{T}"/> indicating whether the status update was successful or containing an error.</returns>
    Task<ErrorOr<Updated>> UpdateUserAccountStatusAsync(
        string userId,
        AccountStatus newStatus,
        CancellationToken cancellationToken = default);
}
