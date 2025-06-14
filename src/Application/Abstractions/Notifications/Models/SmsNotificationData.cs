using RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;

namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Models;

public class SmsNotificationData
{
    /// <summary>
    /// Gets or sets the use case or context of the SMS notification (e.g., System2faOtp).
    /// Required to provide context for the notification's purpose.
    /// </summary>
    public required NotificationUseCase UseCase { get; set; }

    /// <summary>
    /// Gets or sets the list of phone numbers to receive the SMS.
    /// Must contain at least one valid phone number.
    /// </summary>
    public List<string> Receivers { get; set; } = new();

    /// <summary>
    /// Gets or sets the plain text content of the SMS.
    /// Required and must be concise (typically under 160 characters for standard SMS).
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier or name of the user or service that setiated the notification.
    /// Defaults to "System" if not specified.
    /// </summary>
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// Gets or sets the timestamp when the notification was created (UTC).
    /// Automatically set to current UTC time upon creation.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the priority of the SMS notification (e.g., High for OTP).
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Gets or sets the language code for the SMS content (e.g., "en-US" for English).
    /// Used for localization of content.
    /// </summary>
    public string Language { get; set; } = "en-US";

    /// <summary>
    /// Validates the SmsNotificationData instance to ensure it is ready to be sent.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if required fields are missing or invalid.</exception>
    public void Validate()
    {
        if (UseCase == NotificationUseCase.None)
            throw new InvalidOperationException("Notification use case must be specified.");
        if (!Receivers.Any(r => !string.IsNullOrWhiteSpace(r)))
            throw new InvalidOperationException("At least one valid phone number is required.");
        if (string.IsNullOrWhiteSpace(Content))
            throw new InvalidOperationException("Content is required for SMS notifications.");
        if (Content.Length > 160)
            throw new InvalidOperationException("SMS content exceeds 160 characters, which may be truncated by some carriers.");
    }
}
