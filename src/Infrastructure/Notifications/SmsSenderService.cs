using System.Text.RegularExpressions;
using ErrorOr;
using Microsoft.Extensions.Options;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Models;
using RuanFa.FashionShop.Infrastructure.Settings;
using Serilog;

namespace RuanFa.FashionShop.Infrastructure.Notifications;

public interface ISmsSenderService
{
    public Task<ErrorOr<Success>> AddSmsNotificationAsync(SmsNotificationData notificationData, CancellationToken cancellationToken = default);
}

internal class SmsSenderService(IOptions<SmsSettings> smsSettings) : ISmsSenderService
{
    private readonly SmsSettings _smsSettings = smsSettings.Value;

    public async Task<ErrorOr<Success>> AddSmsNotificationAsync(SmsNotificationData notificationData, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate using SmsNotificationData's Validate method
            notificationData.Validate();

            // Validate phone numbers (basic format check)
            foreach (var recipient in notificationData.Receivers)
            {
                if (!IsValidPhoneNumber(recipient))
                {
                    return Error.Validation("SmsNotification.InvalidPhoneNumber", $"Invalid phone number: {recipient}");
                }
            }

            // Log priority and language for tracking
            Log.Information("Sending SMS notification with UseCase: {UseCase}, Priority: {Priority}, Language: {Language} to {Receivers}",
                notificationData.UseCase, notificationData.Priority, notificationData.Language, notificationData.Receivers);

            // Placeholder for actual SMS sending logic
            // TODO: Implement SMS sending logic using a provider like Twilio, Nexmo, etc.
            Log.Debug("SmsSenderService: Sending SMS to {Receivers} with content: {Content}", notificationData.Receivers, notificationData.Content);

            // Simulate a delay for sending
            await Task.Delay(100, cancellationToken);

            Log.Information("SMS notification sent successfully to {Receivers}", notificationData.Receivers);
            return Result.Success;
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, "Validation error in SmsSenderService.AddSmsNotificationAsync");
            return Error.Validation("SmsNotification.ValidationError", ex.Message);
        }
        catch (OperationCanceledException)
        {
            Log.Warning("SMS sending was canceled");
            return Error.Unexpected("SmsNotification.Canceled", "SMS sending was canceled.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error in SmsSenderService.AddSmsNotificationAsync");
            return Error.Unexpected("SmsNotification.Internal", ex.Message);
        }
    }

    private bool IsValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Regex: Allows optional "+" at the start, followed by 10 to 15 digits
        var phoneRegex = new Regex(@"^\+?\d{10,15}$");

        return phoneRegex.IsMatch(phoneNumber);
    }
}
