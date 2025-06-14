namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;

public enum NotificationUseCase
{
    None = 0, // Default value indicating no specific use case

    // System notifications
    SystemActiveEmail,               // Account activation email sent to verify a new user's email address
    SystemResetPassword,             // Password reset email sent when a user requests to reset their password
    System2faOtp,                   // Two-factor authentication OTP sent for additional login security
    SystemOrderConfirmation,        // Order confirmation email sent after a successful order placement
    SystemOrderShipped,             // Order shipped email sent with tracking information when an order is dispatched
    SystemOrderFailed,              // Order failed email sent when an order cannot be processed
    SystemAccountUpdate,            // Account update email sent when a user's account information is modified
    SystemPromotionEmail,           // General promotional email sent for non-specific marketing campaigns

    // User notifications
    UserWelcomeEmail,               // Welcome email sent to new users upon successful registration
    UserProfileUpdateEmail,         // Profile update notification sent when a user updates their profile details
    UserPasswordChangeNotification, // Password change notification sent to confirm a password update

    // Payment notifications
    PaymentSuccessEmail,            // Payment success email sent to confirm a successful transaction
    PaymentFailureEmail,            // Payment failure email sent when a payment attempt fails
    PaymentRefundNotification,      // Payment refund notification sent when a refund is processed

    // Marketing notifications
    MarketingNewsletter,            // Newsletter email sent to subscribers with company updates and promotions
    MarketingDiscountOffer,         // Discount offer email sent to promote a specific sale or promo code
    MarketingSurvey,                // Customer survey invitation email sent to collect user feedback

    // Fashion-specific notifications
    NewCollectionLaunch,            // Notification sent to announce a new fashion collection
    FlashSaleNotification,          // Time-sensitive notification sent to alert users about a flash sale
    BackInStockNotification,        // Notification sent when a previously out-of-stock item is available
    LoyaltyRewardEarned,            // Notification sent when a user earns loyalty points
    AbandonedCartReminder,          // Reminder email sent to users who left items in their cart
    WishlistItemOnSale              // Notification sent when a wishlisted item is discounted
}
