using ErrorOr;

public static partial class DomainErrors
{
    public static class Common
    {
        public static class Address
        {
            public static class Validation
            {
                public static Error AddressesRequired => Error.Validation(
                    code: "Address.AddressesRequired",
                    description: "At least one address is required.");

                public static Error AddressLine1Required => Error.Validation(
                    code: "Address.AddressLine1Required",
                    description: "Address Line 1 is required.");

                public static Error CityRequired => Error.Validation(
                    code: "Address.CityRequired",
                    description: "City is required.");

                public static Error StateRequired => Error.Validation(
                    code: "Address.StateRequired",
                    description: "State is required.");

                public static Error CountryRequired => Error.Validation(
                    code: "Address.CountryRequired",
                    description: "Country is required.");

                public static Error PostalCodeRequired => Error.Validation(
                    code: "Address.PostalCodeRequired",
                    description: "Postal code is required.");

                public static Error InvalidPostalCodeFormat => Error.Validation(
                    code: "Address.InvalidPostalCodeFormat",
                    description: "Postal code format is invalid.");
            }
        }
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
        }
        public static class Email
        {
            public static class Validation
            {
                public static Error InvalidEmailFormat => Error.Validation(
                    code: "Common.InvalidEmailFormat",
                    description: "Please enter a valid email address.");

                public static Error InvalidUsernameFormat => Error.Validation(
                    code: "Common.InvalidUsernameFormat",
                    description: "Username must be 3–20 characters long and use only letters, numbers, or underscores.");
            }
        }
        public static class Password
        {
            public static class Validation
            {
                public static Error PasswordRequired => Error.Validation(
                code: "Common.PasswordRequired",
                description: "Password is required.");

                public static Error PasswordTooShort => Error.Validation(
                    code: "Common.PasswordTooShort",
                    description: "Password must be at least 6 characters long.");

                public static Error InvalidPasswordFormat => Error.Validation(
                    code: "Common.InvalidPasswordFormat",
                    description: "Password must include at least one number, one letter, and one special character.");

                public static Error WeakPasswordComplexity => Error.Validation(
                    code: "Common.WeakPasswordComplexity",
                    description: "Password must include at least one uppercase letter for sufficient complexity.");
            }
        }
    }
}
