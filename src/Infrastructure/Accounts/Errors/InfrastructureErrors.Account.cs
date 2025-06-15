using ErrorOr;

namespace RuanFa.FashionShop.Infrastructure.Accounts.Errors;

/// <summary>
/// Defines error conditions for account-related operations in the infrastructure layer.
/// </summary>
public static partial class InfrastructureErrors
{
    /// <summary>
    /// Contains errors specific to account management operations.
    /// </summary>
    public static class Account
    {
        /// <summary>
        /// Gets an error indicating that a user was not found.
        /// </summary>
        public static Error NotFound => Error.NotFound(
            code: "Account.NotFound",
            description: "The specified user could not be found.");

        /// <summary>
        /// Gets an error indicating that the user's email is not confirmed.
        /// </summary>
        public static Error EmailNotConfirmed => Error.Validation(
            code: "Account.EmailNotConfirmed",
            description: "The email address has not been verified. Please confirm your email.");

        /// <summary>
        /// Gets an error indicating that the account is temporarily locked.
        /// </summary>
        public static Error AccountLocked => Error.Unauthorized(
            code: "Account.AccountLocked",
            description: "The account is temporarily locked. Please contact support.");

        /// <summary>
        /// Gets an error indicating that the sign-in method is not allowed.
        /// </summary>
        public static Error SignInMethodNotAllowed => Error.Unauthorized(
            code: "Account.SignInMethodNotAllowed",
            description: "The sign-in method is not permitted for this account.");

        /// <summary>
        /// Gets an error indicating that the provided credentials are invalid.
        /// </summary>
        public static Error InvalidIdentifier => Error.Unauthorized(
            code: "Account.InvalidIdentifier",
            description: "The email or password is incorrect. Please try again.");

        /// <summary>
        /// Gets an error indicating an internal failure during authentication.
        /// </summary>
        public static Error AuthenticationInternal => Error.Failure(
            code: "Account.AuthenticationInternal",
            description: "An unexpected error occurred during authentication. Please try again later.");

        /// <summary>
        /// Gets an error indicating that a user with the specified email already exists.
        /// </summary>
        public static Error AlreadyExists => Error.Conflict(
            code: "Account.AlreadyExists",
            description: "A user with this email address already exists.");

        /// <summary>
        /// Gets an error indicating that the username is already taken.
        /// </summary>
        public static Error UsernameTaken => Error.Conflict(
            code: "Account.UsernameTaken",
            description: "The requested username is already in use.");

        /// <summary>
        /// Gets an error indicating an internal failure during account creation.
        /// </summary>
        public static Error CreationInternal => Error.Failure(
            code: "Account.CreationInternal",
            description: "An unexpected error occurred while creating the account.");

        /// <summary>
        /// Gets an error indicating an internal failure during account update.
        /// </summary>
        public static Error UpdateInternal => Error.Failure(
            code: "Account.UpdateInternal",
            description: "An unexpected error occurred while updating the account.");

        /// <summary>
        /// Gets an error indicating that the new email is the same as the current email.
        /// </summary>
        public static Error EmailSame => Error.Validation(
            code: "Account.EmailSame",
            description: "The new email address must be different from the current email.");

        /// <summary>
        /// Gets an error indicating that the new password is the same as the current password.
        /// </summary>
        public static Error PasswordSame => Error.Validation(
            code: "Account.PasswordSame",
            description: "The new password must be different from the current password.");

        /// <summary>
        /// Gets an error indicating that a confirmation email has been sent for email verification.
        /// </summary>
        public static Error EmailConfirmationSent => Error.Custom(
            type: 302,
            code: "Account.EmailConfirmationSent",
            description: "A confirmation email has been sent. Please verify your new email address.");

        /// <summary>
        /// Gets an error indicating an internal failure during email confirmation.
        /// </summary>
        public static Error ConfirmEmailInternal => Error.Failure(
            code: "Account.ConfirmEmailInternal",
            description: "An unexpected error occurred while confirming the email.");

        /// <summary>
        /// Gets an error indicating that username generation failed.
        /// </summary>
        public static Error UsernameGenerationFailed => Error.Conflict(
            code: "Account.UsernameGenerationFailed",
            description: "Failed to generate a unique username after multiple attempts.");

        /// <summary>
        /// Gets an error indicating that the default user role was not found.
        /// </summary>
        public static Error RoleNotFound => Error.NotFound(
            code: "Account.RoleNotFound",
            description: "The default user role is not configured in the system.");

        /// <summary>
        /// Gets an error indicating an internal failure during password reset initiation.
        /// </summary>
        public static Error ForgotPasswordInternal => Error.Failure(
            code: "Account.ForgotPasswordInternal",
            description: "An unexpected error occurred while initiating the password reset.");

        /// <summary>
        /// Gets an error indicating that the user was not found during password reset.
        /// </summary>
        public static Error ResetPasswordUserNotFound => Error.NotFound(
            code: "Account.ResetPasswordUserNotFound",
            description: "The specified user could not be found or the email is not verified.");

        /// <summary>
        /// Gets an error indicating that the password reset token is invalid.
        /// </summary>
        public static Error InvalidResetToken => Error.Unauthorized(
            code: "Account.InvalidResetToken",
            description: "The password reset token is invalid.");

        /// <summary>
        /// Gets an error indicating that the confirmation token is invalid.
        /// </summary>
        public static Error InvalidConfirmationToken => Error.Unauthorized(
            code: "Account.InvalidConfirmationToken",
            description: "The confirmation token is invalid.");

        /// <summary>
        /// Gets an error indicating an internal failure during password reset.
        /// </summary>
        public static Error ResetPasswordInternal => Error.Failure(
            code: "Account.ResetPasswordInternal",
            description: "An unexpected error occurred while resetting the password.");

        /// <summary>
        /// Gets an error indicating that the refresh token is invalid.
        /// </summary>
        public static Error RefreshTokenInvalid => Error.Unauthorized(
            code: "Account.RefreshTokenInvalid",
            description: "The provided refresh token is invalid.");

        /// <summary>
        /// Gets an error indicating that the refresh token has expired.
        /// </summary>
        public static Error RefreshTokenExpired => Error.Unauthorized(
            code: "Account.RefreshTokenExpired",
            description: "The refresh token has expired. Please sign in again.");

        /// <summary>
        /// Gets an error indicating an internal failure during token refresh.
        /// </summary>
        public static Error RefreshTokenInternal => Error.Failure(
            code: "Account.RefreshTokenInternal",
            description: "An unexpected error occurred while refreshing the token.");

        /// <summary>
        /// Gets an error indicating an internal failure during confirmation email resend.
        /// </summary>
        public static Error ResendConfirmationInternal => Error.Failure(
            code: "Account.ResendConfirmationInternal",
            description: "An unexpected error occurred while resending the confirmation email.");

        /// <summary>
        /// Gets an error indicating that the social login token is invalid.
        /// </summary>
        public static Error SocialLoginInvalidToken => Error.Unauthorized(
            code: "Account.SocialLoginInvalidToken",
            description: "The social login token is invalid or could not be verified.");

        /// <summary>
        /// Gets an error indicating an internal failure during social login.
        /// </summary>
        public static Error SocialLoginInternal => Error.Failure(
            code: "Account.SocialLoginInternal",
            description: "An unexpected error occurred during social login.");

        /// <summary>
        /// Gets an error indicating an internal failure during account deletion.
        /// </summary>
        public static Error DeletionInternal => Error.Failure(
            code: "Account.DeletionInternal",
            description: "An unexpected error occurred while deleting the account.");

        /// <summary>
        /// Gets an error indicating an internal failure during account retrieval.
        /// </summary>
        public static Error RetrievalInternal => Error.Failure(
            code: "Account.RetrievalInternal",
            description: "An unexpected error occurred while retrieving account details.");

        /// <summary>
        /// Gets an error indicating an internal failure during credential update.
        /// </summary>
        public static Error UpdateCredentialInternal => Error.Failure(
            code: "Account.UpdateCredentialInternal",
            description: "An unexpected error occurred while updating account credentials.");

        /// <summary>
        /// Gets an error indicating the user is not authenticated.
        /// </summary>
        public static Error NotAuthenticated => Error.Unauthorized(
            code: "Account.NotAuthenticated",
            description: "User is not authenticated. Please log in to proceed.");
    }

    /// <summary>
    /// Contains errors specific to role management operations.
    /// </summary>
    public static class Role
    {
        /// <summary>
        /// Gets an error indicating that a role was not found.
        /// </summary>
        public static Error NotFound => Error.NotFound(
            code: "Role.NotFound",
            description: "The specified role could not be found.");

        /// <summary>
        /// Gets an error indicating that a role already exists.
        /// </summary>
        public static Error AlreadyExists => Error.Conflict(
            code: "Role.AlreadyExists",
            description: "A role with this name already exists.");

        /// <summary>
        /// Gets an error indicating that the role name is invalid.
        /// </summary>
        public static Error InvalidRoleName => Error.Validation(
            code: "Role.InvalidRoleName",
            description: "The role name is invalid or empty.");

        /// <summary>
        /// Gets an error indicating that the permissions list is invalid.
        /// </summary>
        public static Error InvalidPermissions => Error.Validation(
            code: "Role.InvalidPermissions",
            description: "The provided permissions list is invalid or null.");

        /// <summary>
        /// Gets an error indicating that the roles list is invalid.
        /// </summary>
        public static Error InvalidRoles => Error.Validation(
            code: "Role.InvalidRoles",
            description: "The provided roles list is invalid or empty.");

        /// <summary>
        /// Gets an error indicating that the user already has the role.
        /// </summary>
        public static Error UserAlreadyHasRole => Error.Conflict(
            code: "Role.UserAlreadyHasRole",
            description: "The user is already assigned to this role.");

        /// <summary>
        /// Gets an error indicating that the user does not have the role.
        /// </summary>
        public static Error UserDoesNotHaveRole => Error.Conflict(
            code: "Role.UserDoesNotHaveRole",
            description: "The user is not assigned to this role.");

        /// <summary>
        /// Gets an error indicating an internal failure during role creation.
        /// </summary>
        public static Error CreateInternal => Error.Failure(
            code: "Role.CreateInternal",
            description: "An unexpected error occurred while creating the role.");

        /// <summary>
        /// Gets an error indicating an internal failure during role deletion.
        /// </summary>
        public static Error DeleteInternal => Error.Failure(
            code: "Role.DeleteInternal",
            description: "An unexpected error occurred while deleting the role.");

        /// <summary>
        /// Gets an error indicating an internal failure during role update.
        /// </summary>
        public static Error UpdateInternal => Error.Failure(
            code: "Role.UpdateInternal",
            description: "An unexpected error occurred while updating the role.");

        /// <summary>
        /// Gets an error indicating an internal failure during role retrieval.
        /// </summary>
        public static Error RetrievalInternal => Error.Failure(
            code: "Role.RetrievalInternal",
            description: "An unexpected error occurred while retrieving role details.");

        /// <summary>
        /// Gets an error indicating an internal failure during permission assignment.
        /// </summary>
        public static Error AssignPermissionsInternal => Error.Failure(
            code: "Role.AssignPermissionsInternal",
            description: "An unexpected error occurred while assigning permissions to the role.");

        /// <summary>
        /// Gets an error indicating an internal failure during role assignment to a user.
        /// </summary>
        public static Error AssignRoleInternal => Error.Failure(
            code: "Role.AssignRoleInternal",
            description: "An unexpected error occurred while assigning the role to the user.");

        /// <summary>
        /// Gets an error indicating an internal failure during multiple role assignments to a user.
        /// </summary>
        public static Error AssignRolesInternal => Error.Failure(
            code: "Role.AssignRolesInternal",
            description: "An unexpected error occurred while assigning roles to the user.");

        /// <summary>
        /// Gets an error indicating an internal failure during multiple role assignments to a user.
        /// </summary>
        public static Error AssignUsersInternal => Error.Failure(
            code: "Role.AssignUsersInternal",
            description: "An unexpected error occurred while assigning user to the role.");

        /// <summary>
        /// Gets an error indicating an internal failure during role removal from a user.
        /// </summary>
        public static Error RemoveRoleInternal => Error.Failure(
            code: "Role.RemoveRoleInternal",
            description: "An unexpected error occurred while removing the role from the user.");

        /// <summary>
        /// Gets an error indicating an internal failure during role membership check.
        /// </summary>
        public static Error CheckRoleInternal => Error.Failure(
            code: "Role.CheckRoleInternal",
            description: "An unexpected error occurred while checking user role membership.");
    }
}
