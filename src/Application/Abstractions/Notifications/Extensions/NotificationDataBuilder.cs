using RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Models;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Constants;

namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Extensions;

public static class NotificationDataBuilder
{
    /// <summary>
    /// Creates a new NotificationData instance with the specified use case.
    /// </summary>
    /// <param name="useCase">The notification use case (default: None).</param>
    /// <returns>A new NotificationData instance.</returns>
    public static NotificationData WithUseCase(NotificationUseCase useCase = NotificationUseCase.None)
    {
        var template = NotificationUseCaseTemplatesData.Templates.FirstOrDefault(t => t.UserCase == useCase);
        return new NotificationData
        {
            UseCase = useCase,
            SendMethodType = GetDefaultSendMethod(useCase),
            TemplateFormatType = template?.TemplateFormatType ?? NotificationTemplateFormat.Default,
            Content = template?.TemplateContent,
            HtmlContent = template?.HtmlTemplateContent,
            Title = template?.Name,
            Values = new Dictionary<NotificationParameter, string?>(),
            Receivers = new List<string>(),
            Attachments = new List<string>(),
            CreatedBy = "System",
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Sets the use case for an existing NotificationData instance.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="useCase">The notification use case (default: None).</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithUseCase(this NotificationData notificationData, NotificationUseCase useCase = NotificationUseCase.None)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));

        var template = NotificationUseCaseTemplatesData.Templates.FirstOrDefault(t => t.UserCase == useCase);
        notificationData.UseCase = useCase;
        notificationData.SendMethodType = GetDefaultSendMethod(useCase);
        notificationData.TemplateFormatType = template?.TemplateFormatType ?? NotificationTemplateFormat.Default;
        notificationData.Content = template?.TemplateContent ?? notificationData.Content;
        notificationData.HtmlContent = template?.HtmlTemplateContent ?? notificationData.HtmlContent;
        notificationData.Title = template?.Name ?? notificationData.Title;
        return notificationData;
    }

    /// <summary>
    /// Sets the send method type for the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="sendMethodType">The send method type (e.g., Email, SMS).</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithSendMethodType(this NotificationData notificationData, NotificationSendMethod sendMethodType)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));

        notificationData.SendMethodType = sendMethodType;
        return notificationData;
    }

    /// <summary>
    /// Adds a single parameter and its value to the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="parameter">The notification parameter to add.</param>
    /// <param name="value">The value of the parameter (null allowed).</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData AddParam(this NotificationData notificationData, NotificationParameter parameter, string? value)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));

        notificationData.Values ??= new Dictionary<NotificationParameter, string?>();
        notificationData.Values[parameter] = value; // Update or add
        return notificationData;
    }

    /// <summary>
    /// Adds multiple parameters and their values to the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="values">A dictionary of parameters and their values.</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData AddParams(this NotificationData notificationData, Dictionary<NotificationParameter, string?> values)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        notificationData.Values ??= new Dictionary<NotificationParameter, string?>();
        foreach (var item in values)
        {
            notificationData.Values[item.Key] = item.Value;
        }
        return notificationData;
    }

    /// <summary>
    /// Adds a list of receivers to the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="receivers">A list of receiver identifiers (e.g., email addresses, phone numbers).</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithReceivers(this NotificationData notificationData, List<string> receivers)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (receivers == null || !receivers.Any(r => !string.IsNullOrWhiteSpace(r)))
            return notificationData;

        notificationData.Receivers ??= new List<string>();
        var uniqueReceivers = receivers.Where(r => !string.IsNullOrWhiteSpace(r) && !notificationData.Receivers.Contains(r)).ToList();
        notificationData.Receivers.AddRange(uniqueReceivers);
        return notificationData;
    }

    /// <summary>
    /// Adds a single receiver to the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="receiver">The receiver identifier (e.g., email address, phone number).</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithReceiver(this NotificationData notificationData, string receiver)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (string.IsNullOrWhiteSpace(receiver))
            return notificationData;

        notificationData.Receivers ??= new List<string>();
        if (!notificationData.Receivers.Contains(receiver))
            notificationData.Receivers.Add(receiver);
        return notificationData;
    }

    /// <summary>
    /// Sets the title of the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="title">The title of the notification (e.g., email subject).</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithTitle(this NotificationData notificationData, string title)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty or whitespace.", nameof(title));

        notificationData.Title = title;
        return notificationData;
    }

    /// <summary>
    /// Sets the plain text content of the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="content">The plain text content of the notification.</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithContent(this NotificationData notificationData, string content)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty or whitespace.", nameof(content));

        notificationData.Content = content;
        return notificationData;
    }

    /// <summary>
    /// Sets the HTML content of the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="htmlContent">The HTML content of the notification.</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithHtmlContent(this NotificationData notificationData, string htmlContent)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (string.IsNullOrWhiteSpace(htmlContent))
            throw new ArgumentException("HTML content cannot be empty or whitespace.", nameof(htmlContent));

        notificationData.HtmlContent = htmlContent;
        return notificationData;
    }

    /// <summary>
    /// Sets the creator of the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="createdBy">The identifier of the creator (e.g., system or user ID).</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithCreatedBy(this NotificationData notificationData, string createdBy)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy cannot be empty or whitespace.", nameof(createdBy));

        notificationData.CreatedBy = createdBy;
        return notificationData;
    }

    /// <summary>
    /// Adds a list of attachment URLs or paths to the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="attachments">A list of attachment URLs or file paths.</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithAttachments(this NotificationData notificationData, List<string> attachments)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (attachments == null || !attachments.Any(a => !string.IsNullOrWhiteSpace(a)))
            return notificationData;

        notificationData.Attachments ??= new List<string>();
        var uniqueAttachments = attachments.Where(a => !string.IsNullOrWhiteSpace(a) && !notificationData.Attachments.Contains(a)).ToList();
        notificationData.Attachments.AddRange(uniqueAttachments);
        return notificationData;
    }

    /// <summary>
    /// Sets the priority of the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="priority">The priority level of the notification.</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithPriority(this NotificationData notificationData, NotificationPriority priority)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));

        notificationData.Priority = priority;
        return notificationData;
    }

    /// <summary>
    /// Sets the language of the notification.
    /// </summary>
    /// <param name="notificationData">The NotificationData instance to modify.</param>
    /// <param name="language">The language code (e.g., "en-US").</param>
    /// <returns>The modified NotificationData instance.</returns>
    public static NotificationData WithLanguage(this NotificationData notificationData, string language)
    {
        if (notificationData == null)
            throw new ArgumentNullException(nameof(notificationData));
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language cannot be empty or whitespace.", nameof(language));

        notificationData.Language = language;
        return notificationData;
    }

    /// <summary>
    /// Creates a new SmsNotificationData instance with the specified use case, receivers, and parameters.
    /// </summary>
    /// <param name="useCase">The notification use case.</param>
    /// <param name="receivers">A list of phone numbers.</param>
    /// <param name="parameters">A dictionary of parameters and their values.</param>
    /// <returns>A new SmsNotificationData instance.</returns>
    public static SmsNotificationData CreateSmsNotificationData(NotificationUseCase useCase, List<string> receivers, Dictionary<NotificationParameter, string?> parameters)
    {
        if (receivers == null || !receivers.Any(r => !string.IsNullOrWhiteSpace(r)))
            throw new ArgumentException("At least one valid phone number is required.", nameof(receivers));
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        var template = NotificationUseCaseTemplatesData.Templates.FirstOrDefault(t => t.UserCase == useCase);
        var content = template?.TemplateContent ?? string.Empty;

        var smsData = new SmsNotificationData
        {
            UseCase = useCase,
            Receivers = receivers.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList(),
            Content = content,
            CreatedBy = "System",
            CreatedAt = DateTimeOffset.UtcNow,
            Priority = useCase == NotificationUseCase.System2faOtp ? NotificationPriority.High : NotificationPriority.Normal
        };

        // Replace parameters in content
        if (!string.IsNullOrWhiteSpace(content))
        {
            foreach (var param in parameters)
            {
                var placeholder = $"{{{param.Key}}}";
                smsData.Content = smsData.Content.Replace(placeholder, param.Value ?? string.Empty);
            }
        }

        smsData.Validate();
        return smsData;
    }

    /// <summary>
    /// Creates a new EmailNotificationData instance with the specified use case, receivers, and parameters.
    /// </summary>
    /// <param name="useCase">The notification use case.</param>
    /// <param name="receivers">A list of email addresses.</param>
    /// <param name="parameters">A dictionary of parameters and their values.</param>
    /// <returns>A new EmailNotificationData instance.</returns>
    public static EmailNotificationData CreateEmailNotificationData(NotificationUseCase useCase, List<string> receivers, Dictionary<NotificationParameter, string?> parameters)
    {
        if (receivers == null || !receivers.Any(r => !string.IsNullOrWhiteSpace(r)))
            throw new ArgumentException("At least one valid email address is required.", nameof(receivers));
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        var template = NotificationUseCaseTemplatesData.Templates.FirstOrDefault(t => t.UserCase == useCase);
        var title = template?.Name ?? useCase.ToString();
        var content = template?.TemplateContent ?? string.Empty;
        var htmlContent = template?.HtmlTemplateContent ?? string.Empty;

        var emailData = new EmailNotificationData
        {
            UseCase = useCase,
            Receivers = receivers.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList(),
            Title = title,
            Content = content,
            HtmlContent = htmlContent,
            CreatedBy = "System",
            CreatedAt = DateTimeOffset.UtcNow,
            Attachments = new List<string>(),
            Priority = useCase == NotificationUseCase.SystemResetPassword ? NotificationPriority.High : NotificationPriority.Normal
        };

        // Replace parameters in content and HTML content
        if (!string.IsNullOrWhiteSpace(content))
        {
            foreach (var param in parameters)
            {
                var placeholder = $"{{{param.Key}}}";
                emailData.Content = emailData.Content.Replace(placeholder, param.Value ?? string.Empty);
                emailData.HtmlContent = emailData.HtmlContent?.Replace(placeholder, param.Value ?? string.Empty);
                emailData.Title = emailData.Title.Replace(placeholder, param.Value ?? string.Empty);
            }
        }

        emailData.Validate();
        return emailData;
    }

    /// <summary>
    /// Creates a NotificationData instance with the specified use case, receivers, and parameters, using template defaults.
    /// </summary>
    /// <param name="useCase">The notification use case.</param>
    /// <param name="receivers">A list of receiver identifiers.</param>
    /// <param name="parameters">A dictionary of parameters and their values.</param>
    /// <returns>A new NotificationData instance.</returns>
    public static NotificationData CreateNotificationData(NotificationUseCase useCase, List<string> receivers, Dictionary<NotificationParameter, string?> parameters)
    {
        if (receivers == null || !receivers.Any(r => !string.IsNullOrWhiteSpace(r)))
            throw new ArgumentException("At least one valid receiver is required.", nameof(receivers));
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        var template = NotificationUseCaseTemplatesData.Templates.FirstOrDefault(t => t.UserCase == useCase);
        var notificationData = new NotificationData
        {
            UseCase = useCase,
            SendMethodType = template?.SendMethodType ?? GetDefaultSendMethod(useCase),
            TemplateFormatType = template?.TemplateFormatType ?? NotificationTemplateFormat.Default,
            Content = template?.TemplateContent,
            HtmlContent = template?.HtmlTemplateContent,
            Title = template?.Name ?? useCase.ToString(),
            Receivers = receivers.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList(),
            Values = new Dictionary<NotificationParameter, string?>(parameters),
            CreatedBy = "System",
            CreatedAt = DateTimeOffset.UtcNow,
            Attachments = new List<string>(),
            Priority = GetDefaultPriority(useCase)
        };

        // Ensure required parameters from template are included
        if (template?.ParamValues != null)
        {
            foreach (var requiredParam in template.ParamValues)
            {
                if (!notificationData.Values.ContainsKey(requiredParam))
                    notificationData.Values[requiredParam] = null;
            }
        }

        notificationData.Validate();
        return notificationData;
    }

    /// <summary>
    /// Determines the default send method based on the notification use case.
    /// </summary>
    /// <param name="useCase">The notification use case.</param>
    /// <returns>The default send method type.</returns>
    private static NotificationSendMethod GetDefaultSendMethod(NotificationUseCase useCase)
    {
        return useCase switch
        {
            NotificationUseCase.System2faOtp => NotificationSendMethod.SMS,
            NotificationUseCase.FlashSaleNotification => NotificationSendMethod.PushNotification,
            NotificationUseCase.BackInStockNotification => NotificationSendMethod.PushNotification,
            _ => NotificationSendMethod.Email
        };
    }

    /// <summary>
    /// Determines the default priority based on the notification use case.
    /// </summary>
    /// <param name="useCase">The notification use case.</param>
    /// <returns>The default priority level.</returns>
    private static NotificationPriority GetDefaultPriority(NotificationUseCase useCase)
    {
        return useCase switch
        {
            NotificationUseCase.System2faOtp => NotificationPriority.High,
            NotificationUseCase.SystemResetPassword => NotificationPriority.High,
            NotificationUseCase.FlashSaleNotification => NotificationPriority.High,
            _ => NotificationPriority.Normal
        };
    }
}
