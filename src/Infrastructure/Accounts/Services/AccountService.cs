using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using ErrorOr;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RuanFa.FashionShop.Application.Abstractions.Data;
using RuanFa.FashionShop.Application.Abstractions.Models;
using RuanFa.FashionShop.Application.Abstractions.Models.Options;
using RuanFa.FashionShop.Application.Abstractions.Models.Pagings;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Extensions;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Services;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Claims;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Contexts;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Enums;
using RuanFa.FashionShop.Application.Abstractions.Security.Authentication.Tokens;
using RuanFa.FashionShop.Application.Abstractions.Security.Authorization.Roles;
using RuanFa.FashionShop.Application.Accounts.Models.Requests;
using RuanFa.FashionShop.Application.Accounts.Models.Responses;
using RuanFa.FashionShop.Application.Accounts.Services;
using RuanFa.FashionShop.Domain.Accounts.Entities;
using RuanFa.FashionShop.Domain.Accounts.Enums;
using RuanFa.FashionShop.Domain.Accounts.Extensions;
using RuanFa.FashionShop.Domain.Accounts.ValueObjects;
using RuanFa.FashionShop.Domain.Commons.Enums;
using RuanFa.FashionShop.Domain.Commons.ValueObjects;
using RuanFa.FashionShop.Infrastructure.Accounts.Entities;
using RuanFa.FashionShop.Infrastructure.Accounts.Errors;
using RuanFa.FashionShop.Infrastructure.Accounts.Extensions;
using RuanFa.FashionShop.Infrastructure.Accounts.Models;
using RuanFa.FashionShop.Infrastructure.Settings;
using RuanFa.FashionShop.SharedKernel.Interfaces.System;
using Serilog;

namespace RuanFa.FashionShop.Infrastructure.Accounts.Services;

internal sealed class AccountService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<JwtSettings> jwtOptions,
    IOptions<ClientSettings> clientOptions,
    IOptions<SocialLoginSettings> socialLoginOptions,
    ITokenProvider tokenProvider,
    IDateTimeProvider dateTimeProvider,
    INotificationService notificationService,
    IApplicationDbContext dbContext,
    IHttpClientFactory httpClientFactory,
    IUserContext userContext)
    : IAccountService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly ClientSettings _clientSettings = clientOptions.Value;
    private readonly SocialLoginSettings _socialLoginSettings = socialLoginOptions.Value;

    private const string SystemUser = "System";
    private const string UserRole = DefaultRoles.User;

    // Validation constants
    private const int MinUsernameLength = 3;
    private const int MaxUsernameAttempts = 10;

    // Transform: Sanitize username with compiled regex
    private static readonly Regex UsernameCleanupRegex = new("[^a-z0-9]", RegexOptions.Compiled);

    public async Task<ErrorOr<TokenResult>> AuthenticateAsync(string userIdentifier, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate: User identifier is not empty
            if (string.IsNullOrWhiteSpace(userIdentifier))
                return InfrastructureErrors.Account.InvalidIdentifier;

            // Check: Retrieve user by identifier
            var appUserResult = await FindUserByIdentifierAsync(userIdentifier, cancellationToken);
            if (appUserResult.IsError)
                return appUserResult.Errors;

            // Assign: Extract user
            var appUser = appUserResult.Value;

            // Verify: Email is confirmed
            if (!await userManager.IsEmailConfirmedAsync(appUser))
                return InfrastructureErrors.Account.EmailNotConfirmed;

            // Await: Password sign-in with lockout
            var result = await signInManager.CheckPasswordSignInAsync(appUser, password, lockoutOnFailure: true);

            // Guard: Account is locked
            if (result.IsLockedOut)
                return InfrastructureErrors.Account.AccountLocked;

            // Guard: Sign-in not allowed
            if (result.IsNotAllowed)
                return InfrastructureErrors.Account.SignInMethodNotAllowed;

            // Verify: Sign-in success
            if (!result.Succeeded)
                return InfrastructureErrors.Account.InvalidIdentifier;

            // Generate: Access and refresh tokens
            var tokenResult = await GenerateAuthenticationResultAsync(appUser);

            // Log: Successful authentication
            Log.Information("User {UserId} authenticated successfully", appUser.Id);
            return tokenResult;
        }
        catch (Exception ex)
        {
            // Log: Authentication error
            Log.Error(ex, "Authentication failed for identifier {UserIdentifier}", userIdentifier);
            // Escalate: Internal error
            return InfrastructureErrors.Account.AuthenticationInternal;
        }
    }

    public async Task<ErrorOr<Success>> ConfirmEmailAsync(string appUserId, string code, string? changedEmail = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user by ID
            var applicationUser = await userManager.FindByIdAsync(appUserId);
            if (applicationUser is null)
                return InfrastructureErrors.Account.NotFound;

            // Check: Retrieve user profile
            var userProfile = await dbContext.Profiles
                .FirstOrDefaultAsync(u => u.UserId == applicationUser.Id, cancellationToken);
            if (userProfile is null)
                return DomainErrors.UserProfile.Resource.NotFound;

            // Parse: Decode confirmation token
            var decodedTokenResult = DecodeToken(code);
            if (decodedTokenResult.IsError)
                return decodedTokenResult.Errors;

            // Assign: Extract decoded token
            var decodedToken = decodedTokenResult.Value;

            IdentityResult result;
            if (string.IsNullOrEmpty(changedEmail))
            {
                // Await: Confirm user email
                result = await userManager.ConfirmEmailAsync(applicationUser, decodedToken);
            }
            else
            {
                // Await: Change user email
                result = await userManager.ChangeEmailAsync(applicationUser, changedEmail, decodedToken);
                if (result.Succeeded)
                {
                    // Update: User email and profile
                    var updateResult = await UpdateUserEmailAndProfile(applicationUser, changedEmail, userProfile, cancellationToken);
                    if (updateResult.IsError)
                        return updateResult.Errors;
                }
            }

            // Verify: Email confirmation success
            if (!result.Succeeded)
                return result.Errors.ToApplicationResult("ConfirmEmailFailed");

            // Log: Email confirmed
            Log.Information("Email confirmed for user {UserId}", appUserId);
            return Result.Success;
        }
        catch (Exception ex)
        {
            // Log: Confirmation error
            Log.Error(ex, "Email confirmation failed for user {UserId}", appUserId);
            // Escalate: Internal error
            return InfrastructureErrors.Account.ConfirmEmailInternal;
        }
    }

    public async Task<ErrorOr<AccountProfieResult>> CreateAccountAsync(RegisterAccountInfo accountInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate: Account creation inputs
            var validationResult = await ValidateAccountCreationAsync(accountInfo);
            if (validationResult.IsError)
                return validationResult.Errors;

            // Generate: Unique username
            var username = await GenerateUniqueUsernameAsync(accountInfo);

            // Create: Identity user
            var account = new ApplicationUser
            {
                UserName = accountInfo.Email,
                Email = accountInfo.Email,
                CreatedAt = dateTimeProvider.UtcNow,
                CreatedBy = SystemUser
            };

            // Create: User with role
            var createResult = await CreateUserWithRoleAsync(account, accountInfo.Password);
            if (createResult.IsError)
                return createResult.Errors;

            // Create: User profile
            var profileResult = await CreateUserProfileAsync(account, accountInfo, username, cancellationToken);
            if (profileResult.IsError)
            {
                // Compensate: Delete account
                await CleanupFailedAccountCreation(account);
                return profileResult.Errors;
            }

            // Check: Retrieve account details
            var accountResult = await GetAccountProfileInternalAsync(account.Id, cancellationToken);
            if (accountResult.IsError)
                return accountResult.Errors;

            // Log: Account creation success
            Log.Information("Account created successfully for user {UserId}", account.Id);
            return accountResult;
        }
        catch (Exception ex)
        {
            // Log: Creation error
            Log.Error(ex, "Failed to create account for email {Email}", accountInfo.Email);
            // Escalate: Internal error
            return InfrastructureErrors.Account.CreationInternal;
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteAccountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return InfrastructureErrors.Account.NotFound;

            // Check: Retrieve user profile
            var userProfile = await dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (userProfile is null)
                return DomainErrors.UserProfile.Resource.NotFound;

            // Await: Delete user
            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                return deleteResult.Errors.ToApplicationResult("DeleteAccountFailed");

            // Await: Delete user profile
            //var deleteProfileResult = dbContext.Profiles.Remove(userProfile);

            // Await: Persist changes
            await dbContext.SaveChangesAsync(cancellationToken);

            // Log: Account deletion success
            Log.Information("Account deleted successfully for user {UserId}", userId);
            return Result.Deleted;
        }
        catch (Exception ex)
        {
            // Log: Deletion error
            Log.Error(ex, "Failed to delete account for user {UserId}", userContext.UserId);
            // Escalate: Internal error
            return InfrastructureErrors.Account.DeletionInternal;
        }
    }

    public async Task<ErrorOr<Success>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user
            var account = await userManager.FindByEmailAsync(email);
            if (account is null || !await userManager.IsEmailConfirmedAsync(account))
                return Result.Success; // Security: Don't reveal if account exists

            // Check: Retrieve user profile
            var profile = await dbContext.Profiles
                .FirstOrDefaultAsync(m => m.UserId == account.Id, cancellationToken);
            if (profile is null)
                return Result.Success;

            // Generate: Password reset token
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(account);
            // Format: Encode reset token
            var encodedResetToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));

            // Send: Password reset email
            var sendEmailResult = await SendPasswordResetEmailAsync(account, profile, email, encodedResetToken, cancellationToken);
            if (sendEmailResult.IsError)
                return sendEmailResult.Errors;

            // Log: Password reset email sent
            Log.Information("Password reset email sent to {Email}", email);
            return Result.Success;
        }
        catch (Exception ex)
        {
            // Log: Password reset error
            Log.Error(ex, "Failed to initiate password reset for email {Email}", email);
            // Escalate: Internal error
            return InfrastructureErrors.Account.ForgotPasswordInternal;
        }
    }

    public async Task<ErrorOr<AccountInfoResult>> GetAccountInfoAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve account details
            return await GetAccountInfoInternalAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: Retrieval error
            Log.Error(ex, "Failed to retrieve account details for user {UserId}", userContext.UserId);
            // Escalate: Internal error
            return InfrastructureErrors.Account.RetrievalInternal;
        }
    }

    public async Task<ErrorOr<PagedList<AccountProfieResult>>> GetAccountProfilesAsync(
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await dbContext.Profiles
                .AsNoTracking()
                .Select(profile => new AccountProfieResult
                {
                    Id = profile.Id,
                    Username = profile.Username,
                    UserId = profile.Id,
                    Email = profile.Email,
                    FullName = profile.FullName,
                    PhoneNumber = profile.PhoneNumber,
                    DateOfBirth = profile.DateOfBirth,
                    Addresses = profile.Addresses,
                    Preferences = profile.Preferences,
                    Wishlist = profile.Wishlist,
                    LoyaltyPoints = profile.LoyaltyPoints,
                    MarketingConsent = profile.MarketingConsent,
                    Gender = profile.Gender,
                })
                .ApplySearch(parameters.Search)
                .ApplySort(parameters.Sort)
                .CreateAsync(parameters.Pagination, cancellationToken);

            return items;
        }
        catch (Exception ex)
        {
            // Log: Retrieval error
            Log.Error(ex, "Failed to retrieve account details for user {UserId}", userContext.UserId);
            // Escalate: Internal error
            return InfrastructureErrors.Account.RetrievalInternal;
        }
    }

    public async Task<ErrorOr<AccountProfieResult>> UpdateAccountAsync(Guid userId, UserProfileInfo updatedProfile, CancellationToken cancellationToken = default)
    {
        try
        {
            // Assign: Current user ID
            var currentUserId = userContext.UserId;
            // Guard: User is authenticated
            if (!currentUserId.HasValue || !userContext.IsAuthenticated)
                return InfrastructureErrors.Account.NotAuthenticated;

            // Check: Retrieve user
            var account = await userManager.FindByIdAsync(userId.ToString());
            if (account is null)
                return InfrastructureErrors.Account.NotFound;

            // Check: Retrieve user profile
            var profile = await dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (profile is null)
                return DomainErrors.UserProfile.Resource.NotFound;

            // Update: User profile details
            var updateResult = profile.UpdatePersonalDetails(
                email: updatedProfile.Email,
                username: updatedProfile.Username,
                fullName: updatedProfile.FullName,
                phoneNumber: updatedProfile.PhoneNumber,
                gender: updatedProfile.Gender,
                dateOfBirth: updatedProfile.DateOfBirth,
                marketingConsent: updatedProfile.MarketingConsent);

            if (updateResult.IsError)
                return updateResult.Errors;

            // Update: User addresses if provided
            if (updatedProfile.Addresses != null)
            {
                var addressResult = profile.UpdateAddresses(updatedProfile.Addresses);
                if (addressResult.IsError)
                    return addressResult.Errors;
            }

            // Update: User preferences if provided
            if (updatedProfile.Preferences != null)
            {
                var preferencesResult = profile.UpdatePreferences(updatedProfile.Preferences);
                if (preferencesResult.IsError)
                    return preferencesResult.Errors;
            }

            // Update: User wishlist if provided
            if (updatedProfile.Wishlist != null)
            {
                var wishlistResult = profile.UpdateWishlist(updatedProfile.Wishlist);
                if (wishlistResult.IsError)
                    return wishlistResult.Errors;
            }

            // Update: Save profile changes
            dbContext.Profiles.Update(profile);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Update: User email if changed
            if (!string.Equals(account.Email, updatedProfile.Email, StringComparison.OrdinalIgnoreCase))
            {
                // Await: Update email
                var emailUpdateResult = await ProcessEmailUpdateAsync(account, profile, updatedProfile.Email, cancellationToken);

                if (emailUpdateResult.IsError)
                    return emailUpdateResult.Errors;
            }

            // Return: Updated profile
            return await GetAccountProfileInternalAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: Update error
            Log.Error(ex, "Failed to update account profile for user {UserId}", userId);
            // Escalate: Internal error
            return InfrastructureErrors.Account.UpdateInternal;
        }
    }


    public async Task<ErrorOr<AccountProfieResult>> GetAccountProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve account details
            return await GetAccountProfileInternalAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: Retrieval error
            Log.Error(ex, "Failed to retrieve account details for user {UserId}", userContext.UserId);
            // Escalate: Internal error
            return InfrastructureErrors.Account.RetrievalInternal;
        }
    }

    public async Task<ErrorOr<TokenResult>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user by refresh token
            var appUser = await userManager.Users
                .SingleOrDefaultAsync(u => u.RefreshToken == token, cancellationToken);
            if (appUser is null)
                return InfrastructureErrors.Account.RefreshTokenInvalid;

            // Verify: Token is not expired
            if (appUser.RefreshTokenExpiryTime <= dateTimeProvider.UtcNow)
                return InfrastructureErrors.Account.RefreshTokenExpired;

            // Generate: New tokens
            var tokenResult = await GenerateAuthenticationResultAsync(appUser);

            // Log: Token refresh success
            Log.Information("Token refreshed successfully for user {UserId}", appUser.Id);
            return tokenResult;
        }
        catch (Exception ex)
        {
            // Log: Token refresh error
            Log.Error(ex, "Failed to refresh token");
            // Escalate: Internal error
            return InfrastructureErrors.Account.RefreshTokenInternal;
        }
    }

    public async Task<ErrorOr<Success>> ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user
            var account = await userManager.FindByEmailAsync(email);
            if (account is null || await userManager.IsEmailConfirmedAsync(account))
                return Result.Success; // Security: Don't reveal if account exists

            // Check: Retrieve user profile
            var profile = await dbContext.Profiles
                .FirstOrDefaultAsync(m => m.UserId == account.Id, cancellationToken);
            if (profile is null)
                return Result.Success;

            // Send: Confirmation email
            var sendEmailResult = await SendConfirmationEmailAsync(account, profile, email, cancellationToken: cancellationToken);
            if (sendEmailResult.IsError)
                return sendEmailResult.Errors;

            // Log: Confirmation email resent
            Log.Information("Confirmation email resent to {Email}", email);
            return Result.Success;
        }
        catch (Exception ex)
        {
            // Log: Resend confirmation error
            Log.Error(ex, "Failed to resend confirmation email to {Email}", email);
            // Escalate: Internal error
            return InfrastructureErrors.Account.ResendConfirmationInternal;
        }
    }

    public async Task<ErrorOr<Success>> ResetPasswordAsync(string email, string resetCode, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Retrieve user
            var user = await userManager.FindByEmailAsync(email);
            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
                return InfrastructureErrors.Account.ResetPasswordUserNotFound;

            // Parse: Decode reset token
            var decodedTokenResult = DecodeToken(resetCode);
            if (decodedTokenResult.IsError)
                return decodedTokenResult.Errors;

            // Await: Reset password
            var result = await userManager.ResetPasswordAsync(user, decodedTokenResult.Value, newPassword);
            if (!result.Succeeded)
                return result.Errors.ToApplicationResult("ResetPasswordFailed");

            // Log: Password reset success
            Log.Information("Password reset successfully for email {Email}", email);
            return Result.Success;
        }
        catch (Exception ex)
        {
            // Log: Password reset error
            Log.Error(ex, "Password reset failed for email {Email}", email);
            // Escalate: Internal error
            return InfrastructureErrors.Account.ResetPasswordInternal;
        }
    }

    public async Task<ErrorOr<TokenResult>> SocialLoginAsync(string provider, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check: Verify social token
            var payload = await VerifySocialTokenAsync(provider, token);
            if (payload is null)
                return InfrastructureErrors.Account.SocialLoginInvalidToken;

            // Check: Existing user by email
            var user = await userManager.FindByEmailAsync(payload.Email);
            if (user is not null)
            {
                // Generate: Tokens for existing user
                var tokenResult = await GenerateAuthenticationResultAsync(user);
                // Log: Social login success
                Log.Information("Social login successful for user {UserId} via {Provider}", user.Id, provider);
                return tokenResult;
            }

            // Create: New social account
            var createResult = await CreateSocialAccountAsync(payload, cancellationToken);
            if (createResult.IsError)
                return createResult.Errors;

            // Check: Retrieve created user
            user = await userManager.FindByEmailAsync(payload.Email);
            if (user is null)
                return InfrastructureErrors.Account.NotFound;

            // Update: Confirm email if verified
            if (payload.IsVerified)
            {
                user.EmailConfirmed = true;
                // Await: Update user
                await userManager.UpdateAsync(user);
            }

            // Generate: Tokens for new user
            var newTokenResult = await GenerateAuthenticationResultAsync(user);
            // Log: Social login account creation
            Log.Information("Social login created new account for email {Email} via {Provider}", payload.Email, provider);

            return newTokenResult;
        }
        catch (Exception ex)
        {
            // Log: Social login error
            Log.Error(ex, "Social login failed for provider {Provider}", provider);
            // Escalate: Internal error
            return InfrastructureErrors.Account.SocialLoginInternal;
        }
    }

    public async Task<ErrorOr<Updated>> UpdateAccountCredentialAsync(
        string? newEmail,
        string? oldPassword,
        string? newPassword,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Assign: Current user ID
            var userId = userContext.UserId;
            // Guard: User is authenticated
            if (!userContext.IsAuthenticated || userId is null)
                return InfrastructureErrors.Account.NotAuthenticated;

            // Check: Retrieve user
            var account = await userManager.FindByIdAsync(userId.Value.ToString());
            if (account is null)
                return InfrastructureErrors.Account.NotFound;

            // Check: Retrieve user profile
            var profile = await dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (profile is null)
                return DomainErrors.UserProfile.Resource.NotFound;

            // Create: Error collection
            var errors = new List<Error>();
            var emailUpdated = false;

            // Validate: New email if provided
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                // Update: Process email change
                var emailUpdateResult = await ProcessEmailUpdateAsync(account, profile, newEmail, cancellationToken);
                if (emailUpdateResult.IsError)
                    errors.AddRange(emailUpdateResult.Errors);
                else
                    emailUpdated = emailUpdateResult.Value;
            }

            // Validate: Password change if provided
            if (!string.IsNullOrWhiteSpace(oldPassword) && !string.IsNullOrWhiteSpace(newPassword))
            {
                // Update: Process password change
                var passwordUpdateResult = await ProcessPasswordUpdateAsync(account, oldPassword, newPassword);
                if (passwordUpdateResult.IsError)
                    errors.AddRange(passwordUpdateResult.Errors);
            }

            // Verify: No errors occurred
            if (errors.Count != 0)
                return errors;

            // Log: Credential update success
            Log.Information("Credentials updated for user {UserId}. Email updated: {EmailUpdated}", userId, emailUpdated);

            return emailUpdated
                ? InfrastructureErrors.Account.EmailConfirmationSent
                : Result.Updated;
        }
        catch (Exception ex)
        {
            // Log: Credential update error
            Log.Error(ex, "Failed to update credentials for user {UserId}", userContext.UserId);
            // Escalate: Internal error
            return InfrastructureErrors.Account.UpdateCredentialInternal;
        }
    }

    #region Private Methods

    private async Task<string> GenerateUniqueUsernameAsync(SocialPayload payload)
    {
        // Generate: Base username from social data
        var baseUsername = !string.IsNullOrWhiteSpace(payload.FirstName)
            ? $"{payload.FirstName}{payload.LastName}".Trim().ToLowerInvariant()
            : payload.Email.Split('@')[0];

        // Generate: Unique username
        return await GenerateUniqueUsernameFromBaseAsync(baseUsername);
    }

    private async Task<string> GenerateUniqueUsernameAsync(RegisterAccountInfo profile)
    {
        // Generate: Base username from profile
        var baseUsername = !string.IsNullOrWhiteSpace(profile.FullName)
            ? profile.FullName.Replace(" ", "").ToLowerInvariant()
            : profile.Email.Split('@')[0];

        // Generate: Unique username
        return await GenerateUniqueUsernameFromBaseAsync(baseUsername);
    }

    private async Task<string> GenerateUniqueUsernameFromBaseAsync(string baseUsername)
    {
        // Transform: Sanitize username
        var cleanUsername = UsernameCleanupRegex.Replace(baseUsername, "");

        // Validate: Ensure minimum length
        if (cleanUsername.Length < MinUsernameLength)
            cleanUsername = "user";

        // Check: Find available username
        for (var i = 1; i <= MaxUsernameAttempts; i++)
        {
            var candidate = i == 1 ? cleanUsername : $"{cleanUsername}{i - 1}";
            if (await userManager.FindByNameAsync(candidate) == null)
                return candidate;
        }

        // Fallback: Use timestamp
        return $"{cleanUsername}{dateTimeProvider.UtcNow:yyyyMMddHHmmss}";
    }

    private async Task<ErrorOr<AccountInfoResult>> GetAccountInfoInternalAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Check: Retrieve user
        var appUser = await userManager.FindByIdAsync(userId.ToString());
        if (appUser is null)
            return InfrastructureErrors.Account.NotFound;

        // Check: Retrieve user profile
        var userProfile = await dbContext.Profiles
            .FirstOrDefaultAsync(m => m.UserId == appUser.Id, cancellationToken);
        if (userProfile is null)
            return DomainErrors.UserProfile.Resource.NotFound;

        // Check: Retrieve permissions
        var permissionSet = await GetPermissionSetAsync(appUser);

        // Create: Account info result
        return new AccountInfoResult(
            UserId: appUser.Id,
            Email: userProfile.Email,
            FullName: userProfile.FullName,
            IsEmailVerified: appUser.EmailConfirmed,
            Created: userProfile.CreatedAt,
            LastLogin: appUser.LastModified,
            Roles: permissionSet.Roles,
            Permisions: permissionSet.Permissions);
    }

    private async Task<ErrorOr<AccountProfieResult>> GetAccountProfileInternalAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Check: Retrieve user
        var appUser = await userManager.FindByIdAsync(userId.ToString());
        if (appUser is null)
            return InfrastructureErrors.Account.NotFound;

        // Check: Retrieve user profile
        var userProfile = await dbContext.Profiles
            .FirstOrDefaultAsync(m => m.UserId == appUser.Id, cancellationToken);
        if (userProfile is null)
            return DomainErrors.UserProfile.Resource.NotFound;

        // Create: Account info result
        return new AccountProfieResult
        {
            Id = userProfile.Id,
            Username = userProfile.Username,
            UserId = appUser.Id,
            Email = userProfile.Email,
            FullName = userProfile.FullName,
            PhoneNumber = userProfile.PhoneNumber,
            DateOfBirth = userProfile.DateOfBirth,
            Addresses = userProfile.Addresses,
            Preferences = userProfile.Preferences,
            Wishlist = userProfile.Wishlist,
            LoyaltyPoints = userProfile.LoyaltyPoints,
            MarketingConsent = userProfile.MarketingConsent,
            Gender = userProfile.Gender,
        };
    }

    private async Task<ErrorOr<ApplicationUser>> FindUserByIdentifierAsync(string identifier, CancellationToken cancellationToken)
    {
        // Check: Find user by email
        var appUser = await userManager.FindByEmailAsync(identifier);
        if (appUser is not null)
            return appUser;

        // Check: Find user by username
        appUser = await userManager.FindByNameAsync(identifier);
        if (appUser is not null)
            return appUser;

        // Escalate: User not found
        return InfrastructureErrors.Account.NotFound;
    }

    private async Task<TokenResult> GenerateAuthenticationResultAsync(ApplicationUser user)
    {
        // Check: Refresh token validity
        if (string.IsNullOrEmpty(user.RefreshToken) || user.RefreshTokenExpiryTime <= dateTimeProvider.UtcNow)
        {
            // Generate: New refresh token
            user.RefreshToken = tokenProvider.CreateRefreshToken(user.Id.ToString());
            // Assign: Token expiry
            user.RefreshTokenExpiryTime = dateTimeProvider.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            // Await: Update user
            await userManager.UpdateAsync(user);
        }

        // Generate: Access token
        var accessToken = tokenProvider.CreateAccessToken(user.Id.ToString(), user.UserName ?? string.Empty, user.Email);

        // Create: Token result
        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken,
            ExpiresIn = (long)TimeSpan.FromMinutes(_jwtSettings.TokenExpirationInMinutes).TotalSeconds
        };
    }

    private async Task<ErrorOr<Success>> SendConfirmationEmailAsync(
        ApplicationUser user,
        UserProfile userProfile,
        string email,
        bool isChange = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate: Confirmation token
            var code = isChange
                ? await userManager.GenerateChangeEmailTokenAsync(user, email)
                : await userManager.GenerateEmailConfirmationTokenAsync(user);

            // Format: Encode token
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            // Check: Retrieve user ID
            var userId = await userManager.GetUserIdAsync(user);

            // Create: Route values for URL
            var routeValues = new List<KeyValuePair<string, string?>>
            {
                new("userId", userId),
                new("code", encodedCode)
            };

            // Add: Changed email if applicable
            if (isChange)
                routeValues.Add(new("changedEmail", email));

            // Format: Confirmation URL
            var confirmEmailUrl = $"{_clientSettings.ClientUri}/confirm-email?{QueryString.Create(routeValues)}";

            // Create: Notification data
            var notificationData = NotificationDataBuilder
                .WithUseCase(NotificationUseCase.SystemActiveEmail)
                .AddParam(NotificationParameter.ActiveUrl, HtmlEncoder.Default.Encode(confirmEmailUrl))
                .AddParam(NotificationParameter.UserFirstName, userProfile.FullName.GetFirstName())
                .WithReceiver(email);

            // Await: Send notification
            var sentNotificationResult = await notificationService.AddNotificationAsync(notificationData, cancellationToken);
            // Verify: Notification success
            return sentNotificationResult.IsError ? sentNotificationResult.Errors : Result.Success;
        }
        catch (Exception ex)
        {
            // Log: Email sending error
            Log.Error(ex, "Failed to send confirmation email to {Email}", email);
            // Escalate: Failure
            return Error.Failure("Account.SendConfirmationEmailFailed", "Failed to send confirmation email.");
        }
    }

    private async Task<ErrorOr<Success>> SendPasswordResetEmailAsync(
        ApplicationUser account,
        UserProfile profile,
        string email,
        string resetCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create: Route values for URL
            var routeValues = new List<KeyValuePair<string, string?>>
            {
                new("userId", await userManager.GetUserIdAsync(account)),
                new("code", resetCode)
            };

            // Format: Reset password URL
            var resetPasswordUrl = $"{_clientSettings.ClientUri}/reset-password?{QueryString.Create(routeValues)}";

            // Create: Notification data
            var notificationData = NotificationDataBuilder
                .WithUseCase(NotificationUseCase.SystemResetPassword)
                .AddParam(NotificationParameter.ResetPasswordUrl, HtmlEncoder.Default.Encode(resetPasswordUrl))
                .AddParam(NotificationParameter.UserFirstName, profile.FullName.GetFirstName())
                .WithReceiver(email);

            // Await: Send notification
            await notificationService.AddNotificationAsync(notificationData, cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            // Log: Email sending error
            Log.Error(ex, "Failed to send password reset email to {Email}", email);
            // Escalate: Failure
            return Error.Failure("Account.SendPasswordResetFailed", "Failed to send password reset email.");
        }
    }

    private async Task<SocialPayload?> VerifySocialTokenAsync(string provider, string token)
    {
        // Check: Select provider
        return provider switch
        {
            SocialProvider.Google => await VerifyGoogleTokenAsync(token),
            SocialProvider.Facebook => await VerifyFacebookTokenAsync(token),
            _ => null
        };
    }

    private async Task<SocialPayload?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            // Create: Validation settings
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_socialLoginSettings.Google.ClientId]
            };

            // Await: Validate token
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            // Create: Social payload
            return new SocialPayload(
                Email: payload.Email,
                IsVerified: payload.EmailVerified,
                ProviderId: payload.Subject,
                FirstName: payload.GivenName,
                LastName: payload.FamilyName,
                PictureUrl: payload.Picture);
        }
        catch (InvalidJwtException ex)
        {
            // Log: Invalid token
            Log.Warning(ex, "Invalid Google token");
            return null;
        }
        catch (Exception ex)
        {
            // Log: Verification error
            Log.Error(ex, "Error verifying Google token");
            // Escalate: Null result
            return null;
        }
    }

    private async Task<SocialPayload?> VerifyFacebookTokenAsync(string accessToken)
    {
        try
        {
            // Acquire: HTTP client
            using var httpClient = httpClientFactory.CreateClient("FacebookAuth");

            // Assign: Facebook app credentials
            var fbAppId = _socialLoginSettings.Facebook.AppId;
            var fbAppSecret = _socialLoginSettings.Facebook.AppSecret;

            // Format: Token verification URL
            var verifyUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={fbAppId}|{fbAppSecret}";

            // Await: Validate token
            var verifyResponse = await httpClient.GetFromJsonAsync<FacebookTokenValidationResponse>(verifyUrl);
            if (verifyResponse?.Data == null || !verifyResponse.Data.IsValid)
            {
                // Log: Validation failure
                Log.Warning("Facebook token validation failed: {Error}", verifyResponse?.Data?.Error?.Message ?? "Unknown error");
                return null;
            }

            // Verify: App ID matches
            if (verifyResponse.Data.AppId != fbAppId)
            {
                // Log: App ID mismatch
                Log.Warning("Facebook token is not for this app. Expected: {Expected}, Got: {Actual}", fbAppId, verifyResponse.Data.AppId);
                return null;
            }

            // Format: User data URL
            var userDataUrl = $"https://graph.facebook.com/v18.0/me?fields=id,first_name,last_name,email,picture&access_token={accessToken}";

            // Await: Retrieve user data
            var userData = await httpClient.GetFromJsonAsync<FacebookUserData>(userDataUrl);
            if (userData == null || string.IsNullOrEmpty(userData.Email))
            {
                // Log: Missing email
                Log.Warning("Could not retrieve user email from Facebook");
                return null;
            }

            // Create: Social payload
            return new SocialPayload(
                Email: userData.Email,
                IsVerified: true,
                ProviderId: userData.Id,
                FirstName: userData.FirstName,
                LastName: userData.LastName,
                PictureUrl: userData.Picture?.Data?.Url);
        }
        catch (Exception ex)
        {
            // Log: Verification error
            Log.Error(ex, "Error verifying Facebook token");
            // Escalate: Null result
            return null;
        }
    }

    private async Task<PermissionSet> GetPermissionSetAsync(ApplicationUser user)
    {
        // Check: Retrieve user roles
        var roles = (await userManager.GetRolesAsync(user)).ToList();
        // Create: Permissions set
        var permissions = new HashSet<string>();

        // Aggregate: Permissions from roles
        foreach (var roleName in roles)
        {
            // Check: Retrieve role
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is not null)
            {
                // Check: Retrieve role claims
                var claims = await roleManager.GetClaimsAsync(role);
                // Filter: Permission claims
                var rolePermissions = claims
                    .Where(c => c.Type == CustomClaims.Permission)
                    .Select(c => c.Value);

                // Merge: Add to permissions set
                permissions.UnionWith(rolePermissions);
            }
        }

        // Create: Permission set
        return new PermissionSet(roles, permissions.ToList());
    }

    private static ErrorOr<string> DecodeToken(string code)
    {
        try
        {
            // Parse: Decode base64 token
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException ex)
        {
            // Log: Token decode failure
            Log.Error(ex, "Failed to decode token");
            // Escalate: Invalid token
            return InfrastructureErrors.Account.InvalidConfirmationToken;
        }
    }

    private async Task<ErrorOr<Success>> ValidateAccountCreationAsync(RegisterAccountInfo accountInfo)
    {
        // Validate: Email is not empty
        if (string.IsNullOrWhiteSpace(accountInfo.Email))
            return DomainErrors.UserProfile.Validation.EmailRequired;

        // Check: Existing user by email
        var existingUser = await userManager.FindByEmailAsync(accountInfo.Email);
        if (existingUser != null)
            return InfrastructureErrors.Account.AlreadyExists;

        // Verify: Default role exists
        if (!await roleManager.RoleExistsAsync(UserRole))
            return InfrastructureErrors.Account.RoleNotFound;

        return Result.Success;
    }

    private async Task<ErrorOr<Success>> CreateUserWithRoleAsync(ApplicationUser account, string password)
    {
        // Await: Create user account
        var createAccountResult = await userManager.CreateAsync(account, password);
        if (!createAccountResult.Succeeded)
            return createAccountResult.Errors.ToApplicationResult("AccountCreationFailed");

        // Assign: Default user role
        var assignRoleResult = await userManager.AddToRoleAsync(account, UserRole);
        if (!assignRoleResult.Succeeded)
        {
            // Compensate: Delete user
            await userManager.DeleteAsync(account);
            // Escalate: Role assignment failure
            return assignRoleResult.Errors.ToApplicationResult("RoleAssignmentFailed");
        }

        return Result.Success;
    }

    private async Task<ErrorOr<Success>> CreateUserProfileAsync(ApplicationUser account, RegisterAccountInfo accountInfo, string username, CancellationToken cancellationToken)
    {
        // Create: User profile
        var createProfileResult = UserProfile.Create(
            userId: account.Id,
            username: username,
            email: accountInfo.Email,
            fullName: accountInfo.FullName,
            phoneNumber: accountInfo.PhoneNumber,
            gender: accountInfo.Gender,
            dateOfBirth: accountInfo.DateOfBirth,
            addresses: accountInfo.Addresses ?? new List<UserAddress>(),
            preferences: accountInfo.Preferences ?? new FashionPreference(),
            wishlist: accountInfo.Wishlist ?? new List<string>(),
            loyaltyPoints: accountInfo.LoyaltyPoints,
            marketingConsent: accountInfo.MarketingConsent);

        if (createProfileResult.IsError)
            return createProfileResult.Errors;

        // Assign: User ID to profile
        UserProfile profile = createProfileResult.Value;
        profile.SetAccount(account.Id);

        // Add: Profile to database
        dbContext.Profiles.Add(profile);
        // Await: Persist changes
        await dbContext.SaveChangesAsync(cancellationToken);

        // Send: Confirmation email
        var sendEmailResult = await SendConfirmationEmailAsync(account, profile, accountInfo.Email, cancellationToken: cancellationToken);
        if (sendEmailResult.IsError)
        {
            // Compensate: Delete user
            await userManager.DeleteAsync(account);
            return sendEmailResult.Errors;
        }

        return Result.Success;
    }

    private async Task CleanupFailedAccountCreation(ApplicationUser account)
    {
        // Remove: Delete user on failure
        await userManager.DeleteAsync(account);
    }

    private async Task<ErrorOr<bool>> ProcessEmailUpdateAsync(ApplicationUser account, UserProfile profile, string newEmail, CancellationToken cancellationToken)
    {
        // Guard: Email is different
        if (string.Equals(newEmail, account.Email, StringComparison.OrdinalIgnoreCase))
            return InfrastructureErrors.Account.EmailSame;

        // Check: Email is not taken
        var existingUser = await userManager.FindByEmailAsync(newEmail);
        if (existingUser != null)
            return InfrastructureErrors.Account.AlreadyExists;

        // Send: Confirmation email for new email
        var sendEmailResult = await SendConfirmationEmailAsync(account, profile, newEmail, true, cancellationToken);
        if (sendEmailResult.IsError)
            return sendEmailResult.Errors;

        // Create: Email updated flag
        return true; // Email updated, confirmation sent
    }

    private async Task<ErrorOr<Success>> ProcessPasswordUpdateAsync(ApplicationUser account, string oldPassword, string newPassword)
    {
        // Guard: Passwords are different
        if (string.Equals(oldPassword, newPassword, StringComparison.Ordinal))
            return InfrastructureErrors.Account.PasswordSame;

        // Await: Change password
        var result = await userManager.ChangePasswordAsync(account, oldPassword, newPassword);
        if (!result.Succeeded)
            return result.Errors.ToApplicationResult("PasswordChangeFailed");

        return Result.Success;
    }

    private async Task<ErrorOr<Success>> UpdateUserEmailAndProfile(ApplicationUser user, string newEmail, UserProfile profile, CancellationToken cancellationToken)
    {
        // Await: Update username
        var setUsernameResult = await userManager.SetUserNameAsync(user, newEmail);
        if (!setUsernameResult.Succeeded)
        {
            // Compensate: Rollback email change
            await userManager.SetEmailAsync(user, user.Email);
            // Escalate: Username update failure
            return setUsernameResult.Errors.ToApplicationResult("ChangeEmailFailed");
        }

        // Update: User profile email
        var updateProfileResult = profile.UpdatePersonalDetails(
            email: newEmail,
            username: profile.Username,
            fullName: profile.FullName,
            phoneNumber: profile.PhoneNumber,
            gender: profile.Gender,
            dateOfBirth: profile.DateOfBirth,
            marketingConsent: profile.MarketingConsent);

        if (updateProfileResult.IsError)
            return updateProfileResult.Errors;

        // Update: Save profile changes
        dbContext.Profiles.Update(profile);
        // Await: Persist changes
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<Updated>> UpdateUserAccountStatusAsync(string userId, AccountStatus newStatus, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate: User ID is not empty
            if (string.IsNullOrWhiteSpace(userId))
                return InfrastructureErrors.Account.InvalidIdentifier;

            // Check: Retrieve user
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return InfrastructureErrors.Account.NotFound;

            // Check: Retrieve user profile
            var profile = await dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId), cancellationToken);
            if (profile == null)
                return InfrastructureErrors.Account.NotFound;

            // Update: Profile status
            user.Status = newStatus;

            // Update: Database context
            dbContext.Profiles.Update(profile);
            // Await: Persist changes
            await dbContext.SaveChangesAsync(cancellationToken);

            // Log: Status updated
            Log.Information("Updated status for user {UserId} to {Status}", userId, newStatus);
            return Result.Updated;
        }
        catch (Exception ex)
        {
            // Log: Status update error
            Log.Error(ex, "Failed to update status for user {UserId}", userId);
            // Escalate: Internal error
            return InfrastructureErrors.Account.UpdateInternal;
        }
    }

    private async Task<ErrorOr<Success>> CreateSocialAccountAsync(SocialPayload payload, CancellationToken cancellationToken)
    {
        // Generate: Unique username
        var uniqueUsername = await GenerateUniqueUsernameAsync(payload);
        // Generate: Random password
        var randomPassword = Guid.NewGuid().ToString("N");

        // Create: New user account info
        var newUserAccountInfo = new RegisterAccountInfo
        {
            Username = uniqueUsername,
            Email = payload.Email,
            FullName = $"{payload.FirstName} {payload.LastName}".Trim(),
            PhoneNumber = null,
            Gender = GenderType.None,
            DateOfBirth = null,
            Addresses = new List<UserAddress>(),
            Preferences = new FashionPreference(),
            Wishlist = new List<string>(),
            LoyaltyPoints = 0,
            MarketingConsent = false,
            Password = randomPassword,
        };

        // Create: New account
        var accountResult = await CreateAccountAsync(newUserAccountInfo, cancellationToken);
        if (accountResult.IsError)
            return accountResult.Errors;

        return Result.Success;
    }

    #endregion
}
