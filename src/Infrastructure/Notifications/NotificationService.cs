using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Enums;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Extensions;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Models;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Services;
using RuanFa.FashionShop.Infrastructure.Data;
using Serilog;

namespace RuanFa.FashionShop.Infrastructure.Notifications;

internal class NotificationService(
    IEmailSenderService emailSenderService,
    ISmsSenderService smsSenderService,
    IServiceScopeFactory serviceScopeFactory)
    : INotificationService
{
    private readonly ApplicationDbContext _dbContext = serviceScopeFactory.CreateScope()
        .ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    public async Task<ErrorOr<Success>> AddNotificationAsync(NotificationData notificationData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate use case
            if (notificationData.UseCase == NotificationUseCase.None)
            {
                return Error.Validation("NotificationService.InvalidUseCase", "Notification use case must be specified.");
            }

            // Validate receivers
            if (notificationData.Receivers == null || !notificationData.Receivers.Any(r => !string.IsNullOrWhiteSpace(r)))
            {
                return Error.Validation("NotificationService.EmptyReceivers", "At least one valid receiver must be provided.");
            }

            // Validate send method type
            if (notificationData.SendMethodType != NotificationSendMethod.Email && notificationData.SendMethodType != NotificationSendMethod.SMS)
            {
                return Error.Validation("NotificationService.InvalidSendMethod", "Invalid send method type.");
            }

            // Validate contact information
            var contactInfoSet = new HashSet<string>(notificationData.Receivers, StringComparer.OrdinalIgnoreCase);
            var contactField = notificationData.SendMethodType == NotificationSendMethod.Email ? "Email" : "PhoneNumber";

            var existingUsers = await _dbContext.Users
                .Where(m => contactInfoSet.Contains(notificationData.SendMethodType == NotificationSendMethod.Email ? m.Email! : m.PhoneNumber!))
                .ToListAsync(cancellationToken);

            if (existingUsers.Count == 0)
            {
                return Error.NotFound("NotificationService.ContactNotFound", "No valid contacts were found.");
            }

            // Filter valid contacts
            List<string> validContacts = notificationData.SendMethodType switch
            {
                NotificationSendMethod.Email => existingUsers
                    .Where(m => !string.IsNullOrEmpty(m.Email))
                    .Select(m => m.Email!)
                    .ToList(),
                NotificationSendMethod.SMS => existingUsers
                    .Where(m => !string.IsNullOrEmpty(m.PhoneNumber))
                    .Select(m => m.PhoneNumber!)
                    .ToList(),
                _ => new List<string>()
            };

            if (validContacts.Count == 0)
            {
                return Error.Validation("NotificationService.NoValidContacts", "None of the provided contact information is valid.");
            }

            // Update receivers to valid contacts
            notificationData.Receivers = validContacts;

            // Validate notification data
            notificationData.Validate();

            // Map to specific notification type and send
            var sendResult = notificationData.SendMethodType switch
            {
                NotificationSendMethod.Email => await emailSenderService.AddEmailNotificationAsync(
                    notificationData.ToEmailNotificationData(), cancellationToken),
                NotificationSendMethod.SMS => await smsSenderService.AddSmsNotificationAsync(
                    notificationData.ToSmsNotificationData(), cancellationToken),
                _ => Error.Validation("NotificationService.InvalidSendMethod", "The send method type is not supported.")
            };

            return sendResult.IsError ? (ErrorOr<Success>)sendResult.Errors : (ErrorOr<Success>)Result.Success;
        }
        catch (DbUpdateException dbEx)
        {
            Log.Error(dbEx, "Database error in NotificationService.AddNotificationAsync");
            return Error.Unexpected("NotificationService.DatabaseError", "A database error occurred.");
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, "Validation error in NotificationService.AddNotificationAsync");
            return Error.Validation("NotificationService.ValidationError", ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error in NotificationService.AddNotificationAsync");
            return Error.Unexpected("NotificationService.Internal", ex.Message);
        }
    }
}
