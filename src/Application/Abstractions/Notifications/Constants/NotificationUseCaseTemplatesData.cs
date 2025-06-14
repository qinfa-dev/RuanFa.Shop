using RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;

namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Constants;

public class NotificationTemplateValueData
{
    public NotificationUseCase UserCase { get; init; }
    public NotificationSendMethod SendMethodType { get; init; } = NotificationSendMethod.Email;
    public NotificationTemplateFormat TemplateFormatType { get; init; } = NotificationTemplateFormat.Default;
    public List<NotificationParameter> ParamValues { get; init; } = [];
    public required string Name { get; init; }
    public string? TemplateContent { get; init; }
    public string? HtmlTemplateContent { get; init; }
    public string? Description { get; init; }
}


public static class NotificationUseCaseTemplatesData
{
    public static readonly List<NotificationTemplateValueData> Templates =
        [
            // Account Activation Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.SystemActiveEmail,
                Name = "Account Activation Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nWelcome to {SystemName}! Your style journey starts here. Activate your account to unlock exclusive offers and shop the latest trends:\n\n{ActiveUrl}\n\nQuestions? Reach out at {SupportEmail}.\n\nHappy styling!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Welcome to {SystemName}! Your style journey starts here. Activate your account to unlock exclusive offers and shop the latest trends:</p><p><a href='{ActiveUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Activate Your Account</a></p><p>Questions? Reach out at <a href='mailto:{SupportEmail}'>{SupportEmail}</a>.</p><p style='color: #777;'>Happy styling!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.ActiveUrl,
                    NotificationParameter.SystemName,
                    NotificationParameter.SupportEmail,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to users upon registration, inviting them to activate their account with a stylish, branded welcome message."
            },

            // Password Reset Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.SystemResetPassword,
                Name = "Password Reset Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nNeed to reset your password? No worries! Click below to set a new one:\n\n{ResetPasswordUrl}\n\nIf you didn’t request this, contact us at {SupportEmail}.\n\nStay fabulous!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Need to reset your password? No worries! Click below to set a new one:</p><p><a href='{ResetPasswordUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p><p>If you didn’t request this, contact us at <a href='mailto:{SupportEmail}'>{SupportEmail}</a>.</p><p style='color: #777;'>Stay fabulous!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.ResetPasswordUrl,
                    NotificationParameter.SystemName,
                    NotificationParameter.SupportEmail,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to users when they request a password reset, providing a link to securely update their password."
            },

            // Two-Factor Authentication OTP
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.System2faOtp,
                Name = "Two-Factor Authentication OTP",
                TemplateFormatType = NotificationTemplateFormat.Default,
                SendMethodType = NotificationSendMethod.SMS,
                TemplateContent = "Hi {UserFirstName}, your OTP for {SystemName} is {OtpCode}. Use it to complete your login. Didn’t request this? Call {SupportPhone}.",
                HtmlTemplateContent = "Hi {UserFirstName},<br><br>Your OTP for {SystemName} is <b>{OtpCode}</b>. Use it to complete your login.<br><br>Didn’t request this? Call <a href='tel:{SupportPhone}'>{SupportPhone}</a>.",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.OtpCode,
                    NotificationParameter.SystemName,
                    NotificationParameter.SupportPhone
                ],
                Description = "Sent via SMS to provide a one-time password for two-factor authentication during login or sensitive actions."
            },

            // Order Confirmation Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.SystemOrderConfirmation,
                Name = "Order Confirmation Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nYour order #{OrderId} is confirmed! Total: {OrderTotal}\nItems: {OrderItems}\n\nWe’ll let you know when it ships. Shop more at {SiteUrl}!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Your order #{OrderId} is confirmed!</p><p><b>Total:</b> {OrderTotal}<br><b>Items:</b> {OrderItems}</p><p>We’ll let you know when it ships.</p><p><a href='{SiteUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Shop More</a></p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.OrderId,
                    NotificationParameter.OrderTotal,
                    NotificationParameter.OrderItems,
                    NotificationParameter.SystemName,
                    NotificationParameter.SiteUrl
                ],
                Description = "Sent after a successful order placement to confirm the purchase and provide order details."
            },

            // Order Shipped Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.SystemOrderShipped,
                Name = "Order Shipped Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nGreat news! Your order #{OrderId} has shipped. Track it here: {OrderTrackingUrl}\n\nExpected delivery: {DeliveryDate}\n\nStay stylish!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Great news! Your order #{OrderId} has shipped.</p><p><b>Expected Delivery:</b> {DeliveryDate}</p><p><a href='{OrderTrackingUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Track Your Order</a></p><p style='color: #777;'>Stay stylish!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.OrderId,
                    NotificationParameter.OrderTrackingUrl,
                    NotificationParameter.DeliveryDate,
                    NotificationParameter.SystemName
                ],
                Description = "Sent when an order is shipped, providing tracking information and expected delivery details."
            },

            // Order Failed Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.SystemOrderFailed,
                Name = "Order Failed Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nWe’re sorry, your order #{OrderId} failed ({OrderStatus}). Please try again or contact us at {SupportEmail}.\n\nWe’re here to help!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>We’re sorry, your order #{OrderId} failed (<b>{OrderStatus}</b>).</p><p>Please try again or contact us at <a href='mailto:{SupportEmail}'>{SupportEmail}</a>.</p><p style='color: #777;'>We’re here to help!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.OrderId,
                    NotificationParameter.OrderStatus,
                    NotificationParameter.SystemName,
                    NotificationParameter.SupportEmail
                ],
                Description = "Sent when an order fails to process, encouraging the user to retry or contact support."
            },

            // Account Update Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.SystemAccountUpdate,
                Name = "Account Update Email",
                TemplateFormatType = NotificationTemplateFormat.Default,
                TemplateContent = "Hi {UserFirstName},\n\nYour {SystemName} account has been updated. If you didn’t make this change, contact us at {SupportEmail}.\n\nKeep shining!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Your {SystemName} account has been updated.</p><p>If you didn’t make this change, contact us at <a href='mailto:{SupportEmail}'>{SupportEmail}</a>.</p><p style='color: #777;'>Keep shining!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.SystemName,
                    NotificationParameter.SupportEmail
                ],
                Description = "Sent to confirm updates to a user’s account information, with a security notice."
            },

            // Payment Success Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.PaymentSuccessEmail,
                Name = "Payment Success Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nYour payment of {PaymentAmount} for order #{OrderId} was successful. Thank you for shopping with {SystemName}!\n\nExplore more styles: {SiteUrl}",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Your payment of <b>{PaymentAmount}</b> for order #{OrderId} was successful.</p><p>Thank you for shopping with {SystemName}!</p><p><a href='{SiteUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Explore More Styles</a></p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.PaymentAmount,
                    NotificationParameter.OrderId,
                    NotificationParameter.SystemName,
                    NotificationParameter.SiteUrl
                ],
                Description = "Sent to confirm a successful payment and encourage further shopping."
            },

            // Payment Failure Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.PaymentFailureEmail,
                Name = "Payment Failure Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nYour payment of {PaymentAmount} for order #{OrderId} failed ({PaymentStatus}). Please update your payment details at {SiteUrl} or contact {SupportEmail}.\n\nWe’re here to help!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Your payment of <b>{PaymentAmount}</b> for order #{OrderId} failed (<b>{PaymentStatus}</b>).</p><p>Please update your payment details at <a href='{SiteUrl}'>our site</a> or contact <a href='mailto:{SupportEmail}'>{SupportEmail}</a>.</p><p style='color: #777;'>We’re here to help!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.PaymentAmount,
                    NotificationParameter.OrderId,
                    NotificationParameter.PaymentStatus,
                    NotificationParameter.SystemName,
                    NotificationParameter.SiteUrl,
                    NotificationParameter.SupportEmail
                ],
                Description = "Sent when a payment fails, guiding the user to update payment details."
            },

            // Payment Refund Notification
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.PaymentRefundNotification,
                Name = "Payment Refund Notification",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nYour refund of {PaymentAmount} for order #{OrderId} has been processed. Questions? Contact us at {SupportEmail}.\n\nShop again at {SiteUrl}!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Your refund of <b>{PaymentAmount}</b> for order #{OrderId} has been processed.</p><p>Questions? Contact us at <a href='mailto:{SupportEmail}'>{SupportEmail}</a>.</p><p><a href='{SiteUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Shop Again</a></p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.PaymentAmount,
                    NotificationParameter.OrderId,
                    NotificationParameter.SystemName,
                    NotificationParameter.SupportEmail,
                    NotificationParameter.SiteUrl
                ],
                Description = "Sent to confirm a refund, providing details and encouraging further shopping."
            },

            // User Welcome Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.UserWelcomeEmail,
                Name = "User Welcome Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nWelcome to {SystemName}! Explore {UserFavoriteCategory} styles and enjoy exclusive offers. Start shopping: {SiteUrl}\n\nHappy styling!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Welcome to {SystemName}! Explore {UserFavoriteCategory} styles and enjoy exclusive offers.</p><p><a href='{SiteUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Start Shopping</a></p><p style='color: #777;'>Happy styling!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.SystemName,
                    NotificationParameter.UserFavoriteCategory,
                    NotificationParameter.SiteUrl,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to new users to welcome them and encourage exploration of the e-shop."
            },

            // User Profile Update Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.UserProfileUpdateEmail,
                Name = "User Profile Update Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nYour profile at {SystemName} has been updated. Not you? Contact us at {SupportEmail}.\n\nKeep shining!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Your profile at {SystemName} has been updated.</p><p>Not you? Contact us at <a href='mailto:{SupportEmail}'>{SupportEmail}</a>.</p><p style='color: #777;'>Keep shining!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.SystemName,
                    NotificationParameter.SupportEmail
                ],
                Description = "Sent to confirm updates to a user’s profile, with a security notice."
            },

            // User Password Change Notification
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.UserPasswordChangeNotification,
                Name = "User Password Change Notification",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nYour {SystemName} password has been changed. If this wasn’t you, contact {SupportEmail} immediately.\n\nStay secure!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Your {SystemName} password has been changed.</p><p>If this wasn’t you, contact <a href='mailto:{SupportEmail}'>{SupportEmail}</a> immediately.</p><p style='color: #777;'>Stay secure!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.SystemName,
                    NotificationParameter.SupportEmail
                ],
                Description = "Sent to confirm a password change, with a security notice."
            },

            // Marketing Newsletter
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.MarketingNewsletter,
                Name = "Marketing Newsletter",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nDiscover the latest trends and exclusive offers from {SystemName}! Check out {UserFavoriteCategory} styles: {SiteUrl}\n\nStay chic!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Discover the latest trends and exclusive offers from {SystemName}!</p><p>Check out {UserFavoriteCategory} styles:</p><p><a href='{SiteUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Explore Now</a></p><p style='color: #777;'>Stay chic!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.SystemName,
                    NotificationParameter.UserFavoriteCategory,
                    NotificationParameter.SiteUrl,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to subscribers with the latest company news, trends, and promotions."
            },

            // Marketing Discount Offer
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.MarketingDiscountOffer,
                Name = "Marketing Discount Offer",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nSteal the look with {PromoDiscount} off using code {PromoCode}! Shop {UserFavoriteCategory} styles before {ExpiryDateTimeOffset}: {PromoUrl}\n\nDon’t miss out!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Steal the look with <b>{PromoDiscount} off</b> using code <b>{PromoCode}</b>!</p><p>Shop {UserFavoriteCategory} styles before {ExpiryDateTimeOffset}:</p><p><a href='{PromoUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Shop Now</a></p><p style='color: #777;'>Don’t miss out!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.PromoCode,
                    NotificationParameter.PromoDiscount,
                    NotificationParameter.PromoUrl,
                    NotificationParameter.UserFavoriteCategory,
                    NotificationParameter.ExpiryDateTimeOffset,
                    NotificationParameter.SystemName,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to promote a limited-time discount, tailored to the user’s favorite fashion category."
            },

            // Marketing Survey Email
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.MarketingSurvey,
                Name = "Marketing Survey Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nHelp us make {SystemName} better! Share your thoughts on {UserFavoriteCategory} styles: {SurveyUrl}\n\nThank you for your feedback!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Help us make {SystemName} better! Share your thoughts on {UserFavoriteCategory} styles:</p><p><a href='{SurveyUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Take Survey</a></p><p style='color: #777;'>Thank you for your feedback!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.SurveyUrl,
                    NotificationParameter.UserFavoriteCategory,
                    NotificationParameter.SystemName,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to collect user feedback via a survey, tailored to their fashion preferences."
            },

            // New Collection Launch
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.NewCollectionLaunch,
                Name = "New Collection Launch",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nOur {CollectionName} collection just dropped! Discover {UserFavoriteCategory} styles: {CollectionUrl}\n\nBe the first to slay the look!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Our <b>{CollectionName}</b> collection just dropped! Discover {UserFavoriteCategory} styles:</p><p><a href='{CollectionUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Shop the Collection</a></p><p style='color: #777;'>Be the first to slay the look!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.CollectionName,
                    NotificationParameter.CollectionUrl,
                    NotificationParameter.UserFavoriteCategory,
                    NotificationParameter.SystemName,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to announce a new fashion collection, highlighting styles tailored to the user’s preferences."
            },

            // Flash Sale Notification
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.FlashSaleNotification,
                Name = "Flash Sale Notification",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nHurry! Our {CollectionName} flash sale is on! Get up to {PromoDiscount} off until {ExpiryDateTimeOffset}. Shop now: {PromoUrl}\n\nDon’t miss out!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Hurry! Our <b>{CollectionName}</b> flash sale is on! Get up to <b>{PromoDiscount} off</b> until {ExpiryDateTimeOffset}.</p><p><a href='{PromoUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Shop Now</a></p><p style='color: #777;'>Don’t miss out!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.CollectionName,
                    NotificationParameter.PromoDiscount,
                    NotificationParameter.PromoUrl,
                    NotificationParameter.ExpiryDateTimeOffset,
                    NotificationParameter.SystemName,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to alert users about a time-sensitive flash sale, creating urgency to drive purchases."
            },

            // Back In Stock Notification
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.BackInStockNotification,
                Name = "Back In Stock Notification",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nGood news! {RecentProductView} is back in stock at {SystemName}. Grab it now: {RecommendedProductUrl}\n\nHurry, it won’t last long!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Good news! <b>{RecentProductView}</b> is back in stock at {SystemName}.</p><p><a href='{RecommendedProductUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Grab It Now</a></p><p style='color: #777;'>Hurry, it won’t last long!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.RecentProductView,
                    NotificationParameter.RecommendedProductUrl,
                    NotificationParameter.SystemName,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent when a previously out-of-stock item is available, encouraging immediate purchase."
            },

            // Loyalty Reward Earned
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.LoyaltyRewardEarned,
                Name = "Loyalty Reward Earned",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nYou’ve earned {LoyaltyPoints} points at {SystemName}! Redeem them for exclusive rewards: {LoyaltyRewardUrl}\n\nKeep shopping to earn more!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>You’ve earned <b>{LoyaltyPoints} points</b> at {SystemName}!</p><p>Redeem them for exclusive rewards:</p><p><a href='{LoyaltyRewardUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Redeem Now</a></p><p style='color: #777;'>Keep shopping to earn more!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.LoyaltyPoints,
                    NotificationParameter.LoyaltyRewardUrl,
                    NotificationParameter.SystemName,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to notify users of earned loyalty points, encouraging them to redeem rewards."
            },

            // Abandoned Cart Reminder
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.AbandonedCartReminder,
                Name = "Abandoned Cart Reminder",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nYou left some fabulous items in your {SystemName} cart! Complete your purchase: {SiteUrl}\n\nHurry, stock is limited!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>You left some fabulous items in your {SystemName} cart!</p><p><a href='{SiteUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Complete Your Purchase</a></p><p style='color: #777;'>Hurry, stock is limited!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.SystemName,
                    NotificationParameter.SiteUrl,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent to remind users about items left in their cart, encouraging them to complete the purchase."
            },

            // Wishlist Item On Sale
            new NotificationTemplateValueData
            {
                UserCase = NotificationUseCase.WishlistItemOnSale,
                Name = "Wishlist Item On Sale",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hi {UserFirstName},\n\nGreat news! {RecentProductView} on your {SystemName} wishlist is now on sale with {PromoDiscount} off! Shop now: {RecommendedProductUrl}\n\nDon’t wait!",
                HtmlTemplateContent = "<html><body style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'><img src='[BrandLogoUrl]' alt='{SystemName} Logo' style='max-width: 150px;'><h2 style='color: #d81b60;'>Hi {UserFirstName},</h2><p>Great news! <b>{RecentProductView}</b> on your {SystemName} wishlist is now on sale with <b>{PromoDiscount} off</b>!</p><p><a href='{RecommendedProductUrl}' style='background-color: #d81b60; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Shop Now</a></p><p style='color: #777;'>Don’t wait!</p><p><a href='[InstagramUrl]'>Follow us on Instagram</a> | <a href='{UnsubscribeUrl}'>Unsubscribe</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFirstName,
                    NotificationParameter.RecentProductView,
                    NotificationParameter.PromoDiscount,
                    NotificationParameter.RecommendedProductUrl,
                    NotificationParameter.SystemName,
                    NotificationParameter.UnsubscribeUrl
                ],
                Description = "Sent when a wishlisted item is discounted, encouraging immediate purchase."
            }
        ];
}
