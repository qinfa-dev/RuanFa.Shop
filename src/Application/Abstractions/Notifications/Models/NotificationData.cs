using RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;

namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Models;

public class NotificationData
{
    /// <summary>
    /// Gets or sets the use case or context of the notification (e.g., Password Reset, Order Confirmation).
    /// Required to provide context for the notification's purpose.
    /// </summary>
    public required NotificationUseCase UseCase { get; set; }

    /// <summary>
    /// Gets or sets the method used for sending the notification (e.g., Email, SMS, PushNotification).
    /// Defaults to Email unless specified otherwise.
    /// </summary>
    public NotificationSendMethod SendMethodType { get; set; } = NotificationSendMethod.Email;

    /// <summary>
    /// Gets or sets the format type of the notification template (e.g., Default, Html).
    /// Determines how the content is rendered.
    /// </summary>
    public NotificationTemplateFormat TemplateFormatType { get; set; } = NotificationTemplateFormat.Default;

    /// <summary>
    /// Gets or sets a collection of key-value pairs for dynamic parameters (e.g., {UserFirstName} -> "Jane").
    /// Used to inject personalized content into the notification.
    /// </summary>
    public Dictionary<NotificationParameter, string?> Values { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of receivers for the notification (e.g., email addresses for Email, phone numbers for SMS).
    /// Must contain at least one valid receiver.
    /// </summary>
    public List<string> Receivers { get; set; } = new();

    /// <summary>
    /// Gets or sets the title or subject of the notification (e.g., email subject line).
    /// Required for Email notifications.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the plain text content of the notification (e.g., body of an email or SMS).
    /// Required for SMS or plain-text Email notifications.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the HTML content of the notification, used for rich-formatted emails.
    /// Ignored for SMS or non-HTML notifications.
    /// </summary>
    public string? HtmlContent { get; set; }

    /// <summary>
    /// Gets or sets the identifier or name of the user or service that initiated the notification.
    /// Defaults to "System" if not specified.
    /// </summary>
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// Gets or sets the timestamp when the notification was created (UTC).
    /// Automatically set to current UTC time upon creation.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets a list of file attachment URLs or paths for Email notifications.
    /// Ignored for SMS or PushNotification.
    /// </summary>
    public List<string> Attachments { get; set; } = new();

    /// <summary>
    /// Gets or sets the priority of the notification (e.g., High for urgent alerts like OTP).
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Gets or sets the language code for the notification (e.g., "en-US" for English).
    /// Used for localization of content.
    /// </summary>
    public string Language { get; set; } = "en-US";

    /// <summary>
    /// Validates the NotificationData instance to ensure it is ready to be sent.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if required fields are missing or invalid.</exception>
    public void Validate()
    {
        if (UseCase == NotificationUseCase.None)
            throw new InvalidOperationException("Notification use case must be specified.");
        if (!Receivers.Any(r => !string.IsNullOrWhiteSpace(r)))
            throw new InvalidOperationException("At least one valid receiver is required.");
        if (SendMethodType == NotificationSendMethod.Email && string.IsNullOrWhiteSpace(Title))
            throw new InvalidOperationException("Title is required for Email notifications.");
        if (SendMethodType == NotificationSendMethod.SMS && string.IsNullOrWhiteSpace(Content))
            throw new InvalidOperationException("Content is required for SMS notifications.");
        if (SendMethodType == NotificationSendMethod.Email && string.IsNullOrWhiteSpace(Content) && string.IsNullOrWhiteSpace(HtmlContent))
            throw new InvalidOperationException("At least one of Content or HtmlContent is required for Email notifications.");
    }
}
