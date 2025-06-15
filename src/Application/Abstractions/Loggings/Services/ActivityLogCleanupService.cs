using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.Application.Abstractions.Data;

namespace RuanFa.FashionShop.Application.Abstractions.Loggings.Services;

public interface IActivityLogCleanupService
{
    Task DeleteOldLogsAsync(int retentionDays, CancellationToken cancellationToken = default);
}

public class ActivityLogCleanupService(IApplicationDbContext context) : IActivityLogCleanupService
{
    public async Task DeleteOldLogsAsync(int retentionDays, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        var oldLogs = await context.ActivityLogs
            .Where(log => log.Timestamp < cutoffDate)
            .ToListAsync(cancellationToken);

        if (oldLogs.Count > 0)
        {
            context.ActivityLogs.RemoveRange(oldLogs);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
