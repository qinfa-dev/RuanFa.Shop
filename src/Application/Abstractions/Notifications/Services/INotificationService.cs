using ErrorOr;
using RuanFa.FashionShop.Application.Abstractions.Notifications.Models;

namespace RuanFa.FashionShop.Application.Abstractions.Notifications.Services;
public interface INotificationService
{
    Task<ErrorOr<Success>> AddNotificationAsync(NotificationData notification, CancellationToken cancellationToken);
}
