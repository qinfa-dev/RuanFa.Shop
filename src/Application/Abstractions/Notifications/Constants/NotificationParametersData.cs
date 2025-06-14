using RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;

namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Constants;

public class NotificationParameterData
{
    public NotificationParameter Parameter { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string SampleData { get; init; }
}

public static class NotificationParametersData
{
    public static readonly List<NotificationParameterData> Parameters =
        [
            // System-related parameters
            new NotificationParameterData
            {
                Parameter = NotificationParameter.SystemName,
                Name = "System Name",
                Description = "The name of the fashion e-shop or application, used for branding in notifications.",
                SampleData = "TrendyThreads"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.SupportEmail,
                Name = "Support Email",
                Description = "The email address for customer service, provided for user support inquiries.",
                SampleData = "support@trendythreads.com"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.SupportPhone,
                Name = "Support Phone",
                Description = "The phone number for customer service, used for urgent or direct support contact.",
                SampleData = "+1-800-555-1234"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.CustomerSupportLink,
                Name = "Customer Support Link",
                Description = "URL to the customer support page, directing users to help resources.",
                SampleData = "https://trendythreads.com/support"
            },

            // User-related parameters
            new NotificationParameterData
            {
                Parameter = NotificationParameter.UserName,
                Name = "User Name",
                Description = "The username of the user, used for login or identification purposes.",
                SampleData = "fashionista123"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.UserEmail,
                Name = "User Email",
                Description = "The email address of the user, used for communication and notifications.",
                SampleData = "jane.doe@domain.com"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.UserFullName,
                Name = "User Full Name",
                Description = "The full name of the user, used for personalized greetings in notifications.",
                SampleData = "Jane Doe"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.UserFirstName,
                Name = "User First Name",
                Description = "The first name of the user, used for a friendly and personal tone.",
                SampleData = "Jane"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.UserLastName,
                Name = "User Last Name",
                Description = "The last name of the user, used in formal or full-name contexts.",
                SampleData = "Doe"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.UserProfileUrl,
                Name = "User Profile URL",
                Description = "URL to the user's profile page, allowing quick access to account settings.",
                SampleData = "https://trendythreads.com/profile"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.UserFavoriteCategory,
                Name = "User Favorite Category",
                Description = "The user's preferred fashion category, used for personalized recommendations.",
                SampleData = "Dresses"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.OtpCode,
                Name = "OTP Code",
                Description = "One-time password for two-factor authentication, used for secure login or actions.",
                SampleData = "123456"
            },

            // Order-related parameters
            new NotificationParameterData
            {
                Parameter = NotificationParameter.OrderId,
                Name = "Order ID",
                Description = "Unique identifier for the order, used to reference specific purchases.",
                SampleData = "ORD123456"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.OrderDate,
                Name = "Order Date",
                Description = "The date the order was placed, providing context for the purchase timeline.",
                SampleData = "2025-05-31"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.OrderTotal,
                Name = "Order Total",
                Description = "The total cost of the order, including all items and fees.",
                SampleData = "$150.00"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.OrderStatus,
                Name = "Order Status",
                Description = "The current status of the order, such as Processing, Shipped, or Delivered.",
                SampleData = "Shipped"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.OrderTrackingNumber,
                Name = "Order Tracking Number",
                Description = "The tracking number for the order shipment, used for tracking delivery.",
                SampleData = "1Z9999W999999999"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.OrderTrackingUrl,
                Name = "Order Tracking URL",
                Description = "URL to track the shipment, linking to the carrier’s tracking page.",
                SampleData = "https://carrier.com/track/1Z9999W999999999"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.OrderItems,
                Name = "Order Items",
                Description = "List of items in the order, providing details of purchased products.",
                SampleData = "Red Dress, Black Sneakers"
            },

            // Payment-related parameters
            new NotificationParameterData
            {
                Parameter = NotificationParameter.PaymentStatus,
                Name = "Payment Status",
                Description = "The status of the payment, such as Successful or Failed.",
                SampleData = "Successful"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.PaymentAmount,
                Name = "Payment Amount",
                Description = "The amount paid for the order, reflecting the transaction value.",
                SampleData = "$150.00"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.PaymentMethod,
                Name = "Payment Method",
                Description = "The payment method used for the order, such as Credit Card or PayPal.",
                SampleData = "Credit Card"
            },

            // Link-related parameters
            new NotificationParameterData
            {
                Parameter = NotificationParameter.ActiveUrl,
                Name = "Activation URL",
                Description = "URL for activating the user’s account, used during registration.",
                SampleData = "https://trendythreads.com/activate?token=xyz"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.ResetPasswordUrl,
                Name = "Reset Password URL",
                Description = "URL for resetting the user’s password, used in password recovery.",
                SampleData = "https://trendythreads.com/reset?token=xyz"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.UnsubscribeUrl,
                Name = "Unsubscribe URL",
                Description = "URL for unsubscribing from marketing emails, ensuring compliance with regulations.",
                SampleData = "https://trendythreads.com/unsubscribe"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.SurveyUrl,
                Name = "Survey URL",
                Description = "URL for a marketing survey, used to collect user feedback.",
                SampleData = "https://trendythreads.com/survey"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.SiteUrl,
                Name = "Site URL",
                Description = "Base URL of the e-shop, used for general navigation links.",
                SampleData = "https://trendythreads.com"
            },

            // Time-related parameters
            new NotificationParameterData
            {
                Parameter = NotificationParameter.CreatedDateTimeOffset,
                Name = "Created DateTimeOffset",
                Description = "Date and time when the user or order was created, for record-keeping.",
                SampleData = "2025-05-31 09:46:00"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.ExpiryDateTimeOffset,
                Name = "Expiry DateTimeOffset",
                Description = "Expiry date and time for a promotion or offer, creating urgency.",
                SampleData = "2025-06-07 23:59:59"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.DeliveryDate,
                Name = "Delivery Date",
                Description = "Expected or actual delivery date for the order, informing users of timelines.",
                SampleData = "2025-06-05"
            },

            // Promotional parameters
            new NotificationParameterData
            {
                Parameter = NotificationParameter.PromoCode,
                Name = "Promo Code",
                Description = "Promotional code applied to the order, offering discounts or incentives.",
                SampleData = "SUMMER25"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.PromoDiscount,
                Name = "Promo Discount",
                Description = "Discount amount applied via a promo code, such as a percentage or fixed amount.",
                SampleData = "25% off"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.PromoUrl,
                Name = "Promo URL",
                Description = "URL for the promotional offer or sale page, directing users to specific campaigns.",
                SampleData = "https://trendythreads.com/summer-sale"
            },

            // Fashion-specific parameters
            new NotificationParameterData
            {
                Parameter = NotificationParameter.CollectionName,
                Name = "Collection Name",
                Description = "Name of a new or featured fashion collection, used in promotional notifications.",
                SampleData = "Summer Chic Collection"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.CollectionUrl,
                Name = "Collection URL",
                Description = "URL to a specific fashion collection, linking to curated product pages.",
                SampleData = "https://trendythreads.com/collections/summer-chic"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.RecentProductView,
                Name = "Recent Product View",
                Description = "Name or link of a recently viewed product, used for re-engagement.",
                SampleData = "Floral Midi Dress"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.RecommendedProductUrl,
                Name = "Recommended Product URL",
                Description = "URL to a recommended product based on user behavior, enhancing personalization.",
                SampleData = "https://trendythreads.com/products/floral-dress"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.LoyaltyPoints,
                Name = "Loyalty Points",
                Description = "User’s current loyalty points balance, used to encourage reward redemption.",
                SampleData = "150 points"
            },
            new NotificationParameterData
            {
                Parameter = NotificationParameter.LoyaltyRewardUrl,
                Name = "Loyalty Reward URL",
                Description = "URL to redeem loyalty rewards, linking to the rewards program page.",
                SampleData = "https://trendythreads.com/rewards"
            }
        ];
}
