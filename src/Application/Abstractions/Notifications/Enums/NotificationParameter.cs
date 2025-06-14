namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;

public enum NotificationParameter
{
    // System-related parameters
    SystemName,                // Name of the fashion e-shop or application (e.g., "TrendyThreads")
    SupportEmail,              // Support email address for customer service (e.g., "support@trendythreads.com")
    SupportPhone,              // Support phone number for customer service (e.g., "+1-800-555-1234")
    CustomerSupportLink,       // URL to the customer support page (e.g., "https://trendythreads.com/support")

    // User-related parameters
    UserName,                  // Username of the user (e.g., "fashionista123")
    UserEmail,                 // Email address of the user (e.g., "user@domain.com")
    UserFullName,              // Full name of the user (e.g., "Jane Doe")
    UserFirstName,             // First name of the user (e.g., "Jane")
    UserLastName,              // Last name of the user (e.g., "Doe")
    UserProfileUrl,            // URL to the user's profile page (e.g., "https://trendythreads.com/profile")
    UserFavoriteCategory,      // User's favorite fashion category (e.g., "Dresses", "Streetwear")
    OtpCode,                   // One-time password for two-factor authentication (e.g., "123456")

    // Order-related parameters
    OrderId,                   // Unique identifier for the order (e.g., "ORD123456")
    OrderDate,                 // Date the order was placed (e.g., "2025-05-31")
    OrderTotal,                // Total cost of the order (e.g., "$150.00")
    OrderStatus,               // Status of the order (e.g., "Processing", "Shipped")
    OrderTrackingNumber,       // Tracking number for the order shipment (e.g., "1Z9999W999999999")
    OrderTrackingUrl,          // URL to track the shipment (e.g., "https://carrier.com/track/1Z9999W999999999")
    OrderItems,                // List of items in the order (e.g., "Red Dress, Black Sneakers")

    // Payment-related parameters
    PaymentStatus,             // Status of the payment (e.g., "Successful", "Failed")
    PaymentAmount,             // Amount paid for the order (e.g., "$150.00")
    PaymentMethod,             // Payment method used (e.g., "Credit Card", "PayPal")

    // Link-related parameters
    ActiveUrl,                 // URL for activating the user's account (e.g., "https://trendythreads.com/activate?token=xyz")
    ResetPasswordUrl,          // URL for resetting the user's password (e.g., "https://trendythreads.com/reset?token=xyz")
    UnsubscribeUrl,            // URL for unsubscribing from marketing emails (e.g., "https://trendythreads.com/unsubscribe")
    SurveyUrl,                 // URL for a marketing survey (e.g., "https://trendythreads.com/survey")
    SiteUrl,                   // Base URL of the e-shop (e.g., "https://trendythreads.com")

    // Time-related parameters
    CreatedDateTimeOffset,           // Date and time when the user or order was created (e.g., "2025-05-31 09:46:00")
    ExpiryDateTimeOffset,            // Expiry date and time for a promotion or offer (e.g., "2025-06-07 23:59:59")
    DeliveryDate,              // Expected or actual delivery date for the order (e.g., "2025-06-05")

    // Promotional parameters
    PromoCode,                 // Promotional code applied to the order (e.g., "SUMMER25")
    PromoDiscount,             // Discount amount applied via promo code (e.g., "25% off")
    PromoUrl,                  // URL for the promotional offer or sale page (e.g., "https://trendythreads.com/summer-sale")

    // Fashion-specific parameters
    CollectionName,            // Name of a new or featured collection (e.g., "Summer Chic Collection")
    CollectionUrl,             // URL to a specific collection (e.g., "https://trendythreads.com/collections/summer-chic")
    RecentProductView,         // Name or link of a recently viewed product (e.g., "Floral Midi Dress")
    RecommendedProductUrl,     // URL to a recommended product based on user behavior (e.g., "https://trendythreads.com/products/floral-dress")
    LoyaltyPoints,             // User's current loyalty points balance (e.g., "150 points")
    LoyaltyRewardUrl           // URL to redeem loyalty rewards (e.g., "https://trendythreads.com/rewards")
}
