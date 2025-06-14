using ErrorOr;
using FluentEmail.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Models;
using RuanFa.FashionShop.Infrastructure.Settings;
using Serilog;

namespace RuanFa.FashionShop.Infrastructure.Notifications;

public interface IEmailSenderService
{
    Task<ErrorOr<Success>> AddEmailNotificationAsync(
        EmailNotificationData notificationData,
        CancellationToken cancellationToken = default);
}

internal class EmailSenderService : IEmailSenderService
{
    private readonly EmailSettings _emailSettings;
    private readonly IFluentEmail _fluentEmail;

    public EmailSenderService(IOptions<EmailSettings> emailSettings, IFluentEmail fluentEmail)
    {
        _emailSettings = emailSettings.Value;
        _fluentEmail = fluentEmail;
    }

    public async Task<ErrorOr<Success>> AddEmailNotificationAsync(EmailNotificationData notificationData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate using EmailNotificationData's Validate method
            notificationData.Validate();

            // Validate email addresses
            foreach (var recipient in notificationData.Receivers)
            {
                if (!IsValidEmail(recipient))
                {
                    return Error.Validation("EmailNotification.InvalidEmail", $"Invalid email address: {recipient}");
                }
            }

            // Validate attachments
            if (notificationData.Attachments.Any())
            {
                var maxSizeInBytes = _emailSettings.MaxAttachmentSize ?? 25 * 1024 * 1024; // Default to 25MB
                var missingAttachments = notificationData.Attachments.Where(a => !File.Exists(a)).ToList();
                if (missingAttachments.Any())
                {
                    return Error.Validation("EmailNotification.InvalidAttachments", $"The following attachments were not found: {string.Join(", ", missingAttachments)}");
                }

                foreach (var attachment in notificationData.Attachments)
                {
                    var fileInfo = new FileInfo(attachment);
                    if (fileInfo.Length > maxSizeInBytes)
                    {
                        return Error.Validation("EmailNotification.AttachmentSize", $"Attachment {attachment} exceeds the maximum size of {maxSizeInBytes / 1024 / 1024}MB.");
                    }
                }
            }

            // Create the email message
            var email = _fluentEmail
                .SetFrom(_emailSettings.FromEmail, _emailSettings.FromName)
                .To(notificationData.Receivers.Select(m => new FluentEmail.Core.Models.Address(m)))
                .Subject(notificationData.Title)
                .PlaintextAlternativeBody(notificationData.Content)
                .Body(notificationData.HtmlContent, isHtml: true);

            // Add attachments with MIME type detection
            if (notificationData.Attachments.Any())
            {
                var contentTypeProvider = new FileExtensionContentTypeProvider();
                foreach (var attachmentPath in notificationData.Attachments)
                {
                    var attachmentBytes = await File.ReadAllBytesAsync(attachmentPath, cancellationToken);
                    email.Attach(new FluentEmail.Core.Models.Attachment
                    {
                        Filename = Path.GetFileName(attachmentPath),
                        Data = new MemoryStream(attachmentBytes),
                        ContentType = contentTypeProvider.TryGetContentType(attachmentPath, out var contentType)
                            ? contentType
                            : "application/octet-stream"
                    });
                }
            }

            // Log priority and language for tracking
            Log.Information("Sending email notification with UseCase: {UseCase}, Priority: {Priority}, Language: {Language} to {Receivers}",
                notificationData.UseCase, notificationData.Priority, notificationData.Language, notificationData.Receivers);

            // Send the email
            var sendResult = await email.SendAsync(cancellationToken);

            if (!sendResult.Successful)
            {
                Log.Error("Failed to send email notification. Errors: {Errors}", sendResult.ErrorMessages);
                return Error.Unexpected("EmailNotification.SendFailed", $"Failed to send email: {string.Join(", ", sendResult.ErrorMessages)}");
            }

            Log.Information("Email notification sent successfully to {Receivers}", notificationData.Receivers);
            return Result.Success;
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, "Validation error in EmailSenderService.AddEmailNotificationAsync");
            return Error.Validation("EmailNotification.ValidationError", ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error in EmailSenderService.AddEmailNotificationAsync");
            return Error.Unexpected("EmailNotification.InternalError", ex.Message);
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
