using ErrorOr;

public static partial class DomainErrors
{
    // Authentication Flow: Login, Password Management, Refresh Tokens
    public static class Authentication
    {
        public static class Login
        {
            public static class Validation
            {
                public static Error InvalidUserCredential => Error.Validation(
                    code: "Login.InvalidUserCredential",
                    description: "Please enter a valid email address or username.");

                public static Error UserCredentialRequired => Error.Validation(
                    code: "Login.UserCredentialRequired",
                    description: "User credential (email or username) is required.");

                public static Error InvalidEmailFormat => Common.Email.Validation.InvalidEmailFormat;
                public static Error InvalidUsernameFormat => Common.Email.Validation.InvalidUsernameFormat;
                public static Error PasswordRequired => Common.Password.Validation.PasswordRequired;
                public static Error PasswordTooShort => Common.Password.Validation.PasswordTooShort;
                public static Error InvalidPasswordFormat => Common.Password.Validation.InvalidPasswordFormat;
                public static Error WeakPasswordComplexity => Common.Password.Validation.WeakPasswordComplexity;

                public static Error InvalidSessionIdFormat => Error.Validation(
                    code: "Login.InvalidSessionIdFormat",
                    description: "Session ID format is invalid.");
            }

            public static class Business
            {
                public static Error AccountLocked => Error.Conflict(
                    code: "Login.AccountLocked",
                    description: "Account is temporarily locked due to too many failed login attempts.");

                public static Error SessionLimitExceeded => Error.Conflict(
                    code: "Login.SessionLimitExceeded",
                    description: "Maximum number of active sessions exceeded.");
            }

            public static class RateLimiting
            {
                public static Error TooManyLoginAttempts => Error.Conflict(
                    code: "Login.TooManyLoginAttempts",
                    description: "Too many login attempts. Please try again later.");
            }

            public static class System
            {
                public static Error AuthenticationServiceDown => Error.Failure(
                    code: "Login.AuthenticationServiceDown",
                    description: "Authentication service is currently unavailable.");
            }

            public static class ExternalDependency
            {
                public static Error ExternalAuthProviderFailure => Error.Failure(
                    code: "Login.ExternalAuthProviderFailure",
                    description: "External authentication provider failed to respond.");
            }

            public static class Permissions
            {
                public static Error TokenRevoked => Error.Unauthorized(
                    code: "Login.TokenRevoked",
                    description: "Access token has been revoked and cannot be used.");
            }
        }

        public static class Password
        {
            public static class Reset
            {
                public static class Validation
                {
                    public static Error EmailRequired => Error.Validation(
                        code: "PasswordReset.EmailRequired",
                        description: "Email address is required for password reset.");

                    public static Error InvalidEmailFormat => Common.Email.Validation.InvalidEmailFormat;

                    public static Error ResetTokenRequired => Error.Validation(
                        code: "PasswordReset.ResetTokenRequired",
                        description: "Reset token is required.");

                    public static Error InvalidResetTokenFormat => Error.Validation(
                        code: "PasswordReset.InvalidResetTokenFormat",
                        description: "Reset token format is invalid.");

                    public static Error NewPasswordRequired => Error.Validation(
                        code: "PasswordReset.NewPasswordRequired",
                        description: "New password is required.");

                    public static Error NewPasswordTooShort => Common.Password.Validation.PasswordTooShort;
                    public static Error InvalidPasswordFormat => Common.Password.Validation.InvalidPasswordFormat;
                    public static Error WeakPasswordComplexity => Common.Password.Validation.WeakPasswordComplexity;
                }

                public static class RateLimiting
                {
                    public static Error TooManyResetAttempts => Error.Conflict(
                        code: "PasswordReset.TooManyResetAttempts",
                        description: "Too many password reset attempts. Please try again later.");
                }
            }

            public static class Update
            {
                public static class Validation
                {
                    public static Error NewPasswordRequired => Error.Validation(
                        code: "PasswordUpdate.NewPasswordRequired",
                        description: "New password is required.");

                    public static Error OldPasswordRequired => Error.Validation(
                        code: "PasswordUpdate.OldPasswordRequired",
                        description: "Current password is required.");

                    public static Error NewPasswordTooShort => Common.Password.Validation.PasswordTooShort;
                    public static Error InvalidPasswordFormat => Common.Password.Validation.InvalidPasswordFormat;
                    public static Error WeakPasswordComplexity => Common.Password.Validation.WeakPasswordComplexity;
                }

                public static class Business
                {
                    public static Error PasswordsCannotMatch => Error.Validation(
                        code: "PasswordUpdate.PasswordsCannotMatch",
                        description: "New password cannot be the same as current password.");
                }
            }

            public static class Recovery
            {
                public static class Validation
                {
                    public static Error EmailRequired => Error.Validation(
                        code: "PasswordRecovery.EmailRequired",
                        description: "Email address is required for password recovery.");

                    public static Error InvalidEmailFormat => Common.Email.Validation.InvalidEmailFormat;
                }
            }
        }

        public static class RefreshToken
        {
            public static class Validation
            {
                public static Error InvalidRefreshToken => Error.Validation(
                    code: "RefreshToken.InvalidRefreshToken",
                    description: "Refresh token is invalid or expired.");

                public static Error RefreshTokenRequired => Error.Validation(
                    code: "Authentication.RefreshTokenRequired",
                    description: "Refresh token is required.");
            }

            public static class RateLimiting
            {
                public static Error TokenRefreshRateLimited => Error.Conflict(
                    code: "RefreshToken.TokenRefreshRateLimited",
                    description: "Too many refresh token requests. Please try again later.");
            }
        }
    }

    // User Registration Flow
    public static class UserRegistration
    {
        public static class Register
        {
            public static class Validation
            {
                public static Error EmailRequired => Error.Validation(
                    code: "UserAccount.EmailRequired",
                    description: "Email address is required.");

                public static Error InvalidEmailFormat => Common.Email.Validation.InvalidEmailFormat;
                public static Error InvalidUsernameFormat => Common.Email.Validation.InvalidUsernameFormat;
                public static Error PasswordRequired => Common.Password.Validation.PasswordRequired;
                public static Error PasswordTooShort => Common.Password.Validation.PasswordTooShort;
                public static Error InvalidPasswordFormat => Common.Password.Validation.InvalidPasswordFormat;
                public static Error WeakPasswordComplexity => Common.Password.Validation.WeakPasswordComplexity;

                public static Error FullNameRequired => Error.Validation(
                    code: "UserAccount.FullNameRequired",
                    description: "Full name is required.");

                public static Error FullNameTooShort => Error.Validation(
                    code: "UserAccount.FullNameTooShort",
                    description: "Full name must be at least 3 characters long.");

                public static Error InvalidPhoneNumber => Error.Validation(
                    code: "UserAccount.InvalidPhoneNumber",
                    description: "Please enter a valid phone number.");

                public static Error InvalidCountryCode => Error.Validation(
                    code: "UserAccount.InvalidCountryCode",
                    description: "Country code for phone number is invalid.");

                public static Error GenderRequired => Error.Validation(
                    code: "UserAccount.GenderRequired",
                    description: "Gender is required.");

                public static Error InvalidDateOfBirth => Error.Validation(
                    code: "UserAccount.InvalidDateOfBirth",
                    description: "Please enter a valid date of birth.");

                public static Error TermsNotAccepted => Error.Validation(
                    code: "UserAccount.TermsNotAccepted",
                    description: "You must accept the terms and conditions to register.");
            }

            public static class Business
            {
                public static Error MinimumAgeNotMet => Error.Conflict(
                    code: "UserAccount.MinimumAgeNotMet",
                    description: "You must be at least 13 years old to register.");

                public static Error DuplicateEmail => Error.Conflict(
                    code: "UserAccount.DuplicateEmail",
                    description: "This email is already registered to another account.");
            }

            public static class RateLimiting
            {
                public static Error RegistrationRateLimited => Error.Conflict(
                    code: "UserAccount.RegistrationRateLimited",
                    description: "Too many registration attempts from this IP or device. Please try again later.");
            }

            public static class System
            {
                public static Error DatabaseErrorOnRegistration => Error.Failure(
                    code: "UserAccount.DatabaseErrorOnRegistration",
                    description: "Failed to save user data due to database issues.");
            }

            public static class ExternalDependency
            {
                public static Error VerificationServiceFailure => Error.Failure(
                    code: "UserAccount.VerificationServiceFailure",
                    description: "Failed to send verification email due to external service issues.");
            }

            public static class Permissions
            {
                public static Error RegistrationRestricted => Error.Unauthorized(
                    code: "UserAccount.RegistrationRestricted",
                    description: "Registration is disabled for your region or IP.");
            }
        }
    }

    // Email Management Flow
    public static class EmailManagement
    {
        public static class Confirmation
        {
            public static class Validation
            {
                public static Error UserIdRequired => Error.Validation(
                    code: "EmailConfirmation.UserIdRequired",
                    description: "User ID is required.");

                public static Error InvalidUserIdFormat => Error.Validation(
                    code: "EmailConfirmation.InvalidUserIdFormat",
                    description: "User ID format is invalid.");

                public static Error ConfirmationCodeRequired => Error.Validation(
                    code: "EmailConfirmation.ConfirmationCodeRequired",
                    description: "Confirmation code is required.");

                public static Error InvalidConfirmationCodeFormat => Error.Validation(
                    code: "EmailConfirmation.InvalidConfirmationCodeFormat",
                    description: "Confirmation code format is invalid.");

                public static Error InvalidEmailFormat => Common.Email.Validation.InvalidEmailFormat;

                public static Error InvalidVerificationCodeLength => Error.Validation(
                    code: "EmailConfirmation.InvalidVerificationCodeLength",
                    description: "Verification code length is invalid.");
            }

            public static class Business
            {
                public static Error VerificationCodeExpired => Error.Conflict(
                    code: "EmailConfirmation.VerificationCodeExpired",
                    description: "Email verification code has expired.");
            }

            public static class RateLimiting
            {
                public static Error EmailConfirmationRateLimited => Error.Conflict(
                    code: "EmailConfirmation.EmailConfirmationRateLimited",
                    description: "Too many email confirmation attempts. Please try again later.");
            }

            public static class System
            {
                public static Error EmailServiceDown => Error.Failure(
                    code: "EmailConfirmation.EmailServiceDown",
                    description: "Email service is currently unavailable.");
            }

            public static class ExternalDependency
            {
                public static Error EmailDeliveryFailure => Error.Failure(
                    code: "EmailConfirmation.EmailDeliveryFailure",
                    description: "Failed to deliver email due to external email provider issues.");
            }
        }

        public static class ResendConfirmation
        {
            public static class Validation
            {
                public static Error ResendEmailRequired => Error.Validation(
                    code: "EmailResendConfirmation.ResendEmailRequired",
                    description: "Email address is required for resending confirmation.");

                public static Error ResendInvalidEmailFormat => Common.Email.Validation.InvalidEmailFormat;
            }

            public static class Business
            {
                public static Error ResendLimitExceeded => Error.Conflict(
                    code: "EmailResendConfirmation.ResendLimitExceeded",
                    description: "Maximum number of email resend attempts reached.");
            }

            public static class System
            {
                public static Error EmailServiceDown => Error.Failure(
                    code: "EmailResendConfirmation.EmailServiceDown",
                    description: "Email service is currently unavailable.");
            }

            public static class ExternalDependency
            {
                public static Error EmailDeliveryFailure => Error.Failure(
                    code: "EmailResendConfirmation.EmailDeliveryFailure",
                    description: "Failed to deliver email due to external email provider issues.");
            }
        }

        public static class Update
        {
            public static class Validation
            {
                public static Error NewEmailRequired => Error.Validation(
                    code: "EmailUpdate.NewEmailRequired",
                    description: "New email address is required.");

                public static Error InvalidEmailFormat => Common.Email.Validation.InvalidEmailFormat;
            }

            public static class Permissions
            {
                public static Error EmailUpdateRestricted => Error.Unauthorized(
                    code: "EmailUpdate.EmailUpdateRestricted",
                    description: "You are not allowed to change email due to account restrictions.");
            }
        }
    }

    // User Profile Flow
    public static class UserProfile
    {
        public static class UserAddress
        {
            public static class Validation
            {
                public static Error AddressTypeRequired => Error.Validation(
                    code: "UserAddress.AddressTypeRequired",
                    description: "Address type is required.");

                public static Error DefaultAddressFlagInvalid => Error.Validation(
                    code: "UserAddress.DefaultAddressFlagInvalid",
                    description: "IsDefault flag must be a boolean value (true or false).");

                public static Error AddressLineTooLong => Error.Validation(
                    code: "UserAddress.AddressLineTooLong",
                    description: "Address line exceeds maximum character limit.");
            }

            public static class Business
            {
                public static Error MaxAddressesExceeded => Error.Conflict(
                    code: "UserAddress.MaxAddressesExceeded",
                    description: "Maximum number of saved addresses reached.");
            }
        }

        public static class FashionPreferences
        {
            public static class Validation
            {
                public static Error WishlistRequired => Error.Validation(
                    code: "FashionPreferences.WishlistRequired",
                    description: "Wishlist is required.");

                public static Error ClothingSizeRequired => Error.Validation(
                    code: "FashionPreferences.ClothingSizeRequired",
                    description: "Clothing size is required.");

                public static Error InvalidClothingSize => Error.Validation(
                    code: "FashionPreferences.InvalidClothingSize",
                    description: "Clothing size is invalid.");
            }

            public static class Business
            {
                public static Error FavoriteCategoriesRequired => Error.Validation(
                    code: "FashionPreferences.FavoriteCategoriesRequired",
                    description: "At least one favorite category is required.");

                public static Error InvalidCategoryName => Error.Validation(
                    code: "FashionPreferences.InvalidCategoryName",
                    description: "One or more category names are invalid.");
            }

            public static class RateLimiting
            {
                public static Error WishlistUpdateRateLimited => Error.Conflict(
                    code: "FashionPreferences.WishlistUpdateRateLimited",
                    description: "Too many wishlist modifications. Please try again later.");
            }
        }

        public static class Validation
        {
            public static Error UserCredentialRequired => Error.Validation(
                code: "UserAccount.UserCredentialRequired",
                description: "User credential (email or username) is required.");

            public static Error InvalidUserCredential => Error.Validation(
                code: "UserAccount.InvalidUserCredential",
                description: "Please enter a valid email address or username.");

            public static Error EmailRequired => Error.Validation(
                code: "UserAccount.EmailRequired",
                description: "Email address is required.");

            public static Error InvalidEmailFormat => Common.Email.Validation.InvalidEmailFormat;
            public static Error InvalidUsernameFormat => Common.Email.Validation.InvalidUsernameFormat;
            public static Error PasswordRequired => Common.Password.Validation.PasswordRequired;
            public static Error PasswordTooShort => Common.Password.Validation.PasswordTooShort;
            public static Error InvalidPasswordFormat => Common.Password.Validation.InvalidPasswordFormat;
            public static Error WeakPasswordComplexity => Common.Password.Validation.WeakPasswordComplexity;

            public static Error FullNameRequired => Error.Validation(
                code: "UserAccount.FullNameRequired",
                description: "Full name is required.");

            public static Error FullNameTooShort => Error.Validation(
                code: "UserAccount.FullNameTooShort",
                description: "Full name must be at least 3 characters long.");

            public static Error InvalidPhoneNumber => Error.Validation(
                code: "UserAccount.InvalidPhoneNumber",
                description: "Please enter a valid phone number.");

            public static Error GenderRequired => Error.Validation(
                code: "UserAccount.GenderRequired",
                description: "Gender is required.");

            public static Error InvalidDateOfBirth => Error.Validation(
                code: "UserAccount.InvalidDateOfBirth",
                description: "Please enter a valid date of birth.");

            public static Error InvalidUserId => Error.Validation(
                code: "UserAccount.InvalidUserId",
                description: "User ID cannot be empty or invalid.");

            public static Error InvalidProfilePictureFormat => Error.Validation(
                code: "UserAccount.InvalidProfilePictureFormat",
                description: "Profile picture is in an unsupported format.");
        }

        public static class Business
        {
            public static Error InvalidPoints => Error.Validation(
                code: "UserAccount.InvalidPoints",
                description: "Loyalty points must be zero or greater.");

            public static Error InvalidPointsToRedeem => Error.Validation(
                code: "UserAccount.InvalidPointsToRedeem",
                description: "Points to redeem must be greater than zero and not exceed your available balance.");

            public static Error InvalidOrder => Error.Validation(
                code: "UserAccount.InvalidOrder",
                description: "The associated order is invalid or missing.");

            public static Error LoyaltyPointsExpired => Error.Conflict(
                code: "UserAccount.LoyaltyPointsExpired",
                description: "Loyalty points have expired and cannot be used.");
        }

        public static class RateLimiting
        {
            public static Error ProfileUpdateRateLimited => Error.Conflict(
                code: "UserAccount.ProfileUpdateRateLimited",
                description: "Too many profile update attempts. Please try again later.");
        }

        public static class System
        {
            public static Error ProfileSaveFailure => Error.Failure(
                code: "UserAccount.ProfileSaveFailure",
                description: "Failed to save profile changes due to database issues.");
        }

        public static class ExternalDependency
        {
            public static Error ImageUploadServiceFailure => Error.Failure(
                code: "UserAccount.ImageUploadServiceFailure",
                description: "Failed to upload profile picture to external storage service.");
        }

        public static class Resource
        {
            public static Error NotFound => Error.NotFound(
                code: "UserAccount.NotFound",
                description: "The user account was not found.");

            public static Error OrderNotFound => Error.NotFound(
                code: "UserAccount.OrderNotFound",
                description: "The specified order was not found.");
        }

        public static class Permissions
        {
            public static Error UnauthorizedAccess => Error.Unauthorized(
                code: "UserAccount.UnauthorizedAccess",
                description: "You do not have permission to access this account.");

            public static Error ProfileEditRestricted => Error.Unauthorized(
                code: "UserAccount.ProfileEditRestricted",
                description: "You lack permission to edit certain profile fields.");
        }

    }
    // Role Management Flow
    public static class Role
    {
        public static class Validation
        {
            public static Error InvalidName => Error.Validation(
                code: "Role.InvalidName",
                description: "Role name cannot be empty or null.");

            public static Error NameTooShort => Error.Validation(
                code: "Role.NameTooShort",
                description: "Role name must be at least 3 characters long.");

            public static Error NameTooLong => Error.Validation(
                code: "Role.NameTooLong",
                description: "Role name must not exceed 50 characters.");

            public static Error InvalidNameFormat => Error.Validation(
                code: "Role.InvalidNameFormat",
                description: "Role name must contain only letters, numbers, and underscores.");

            public static Error InvalidPermissions => Error.Validation(
                code: "Role.InvalidPermissions",
                description: "Permissions list cannot be null or contain invalid entries.");

            public static Error InvalidAssignedRoles => Error.Validation(
                code: "Role.InvalidAssignedRoles",
                description: "Assign roles list cannot be null or contain invalid entries.");
        }

        public static class Business
        {
            public static Error DuplicateName => Error.Conflict(
                code: "Role.DuplicateName",
                description: "A role with this name already exists.");

            public static Error PermissionNotRecognized => Error.Conflict(
                code: "Role.PermissionNotRecognized",
                description: "One or more permissions are not recognized by the system.");
        }

        public static class Resource
        {
            public static Error NotFound => Error.NotFound(
                code: "Role.NotFound",
                description: "The specified role was not found.");
        }

        public static class Permissions
        {
            public static Error UnauthorizedRoleCreation => Error.Unauthorized(
                code: "Role.UnauthorizedRoleCreation",
                description: "You do not have permission to create roles.");

            public static Error UnauthorizedRoleModification => Error.Unauthorized(
                code: "Role.UnauthorizedRoleModification",
                description: "You do not have permission to modify roles.");
        }

        public static class System
        {
            public static Error RoleSaveFailure => Error.Failure(
                code: "Role.RoleSaveFailure",
                description: "Failed to save role changes due to database issues.");
        }

        public static class ExternalDependency
        {
            public static Error PermissionServiceFailure => Error.Failure(
                code: "Role.PermissionServiceFailure",
                description: "Failed to validate permissions due to external service issues.");
        }

        public static class RateLimiting
        {
            public static Error RoleCreationRateLimited => Error.Conflict(
                code: "Role.RoleCreationRateLimited",
                description: "Too many role creation attempts. Please try again later.");
        }
    }
    // Permissions category (for future extensibility beyond UserProfile)
    public static class Permissions
    {
        // Additional Permissions errors can be added here
    }
}
