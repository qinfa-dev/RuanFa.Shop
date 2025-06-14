using RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;

namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Models;

public class EmailNotificationData
{
    /// <summary>
    /// Gets or sets the use case or context of the email notification (e.g., Order Confirmation).
    /// Required to provide context for the notification's purpose.
    /// </summary>
    public required NotificationUseCase UseCase { get; set; }

    /// <summary>
    /// Gets or sets the list of email addresses to receive the email.
    /// Must contain at least one valid email address.
    /// </summary>
    public List<string> Receivers { get; set; } = new();

    /// <summary>
    /// Gets or sets the subject line of the email.
    /// Required for email notifications.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plain text body of the email.
    /// Required if HtmlBody is not provided.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the HTML body of the email for rich formatting.
    /// Required if Content is not provided.
    /// </summary>
    public string? HtmlContent { get; set; }

    /// <summary>
    /// Gets or sets the identifier or name of the user or service that setiated the notification.
    /// Defaults to "System" if not specified.
    /// </summary>
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// Gets or sets a list of file attachment URLs or paths for the email.
    /// Optional and used for email attachments.
    /// </summary>
    public List<string> Attachments { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp when the notification was created (UTC).
    /// Automatically set to current UTC time upon creation.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the priority of the email notification (e.g., High for urgent alerts).
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Gets or sets the language code for the email content (e.g., "en-US" for English).
    /// Used for localization of content.
    /// </summary>
    public string Language { get; set; } = "en-US";

    /// <summary>
    /// Validates the EmailNotificationData instance to ensure it is ready to be sent.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if required fields are missing or invalid.</exception>
    public void Validate()
    {
        if (UseCase == NotificationUseCase.None)
            throw new InvalidOperationException("Notification use case must be specified.");
        if (!Receivers.Any(r => !string.IsNullOrWhiteSpace(r)))
            throw new InvalidOperationException("At least one valid email address is required.");
        if (string.IsNullOrWhiteSpace(Title))
            throw new InvalidOperationException("Title is required for email notifications.");
        if (string.IsNullOrWhiteSpace(Content) && string.IsNullOrWhiteSpace(HtmlContent))
            throw new InvalidOperationException("At least one of Content or HtmlContent is required for email notifications.");
    }
}
