namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;
public enum NotificationSendMethod
{
    None = 0,
    Email = 1,
    SMS = 2,
    PushNotification = 3, // For mobile app notifications (e.g., new arrivals, order updates)
    WhatsApp = 4         // Popular for fashion brands in certain regions
}
