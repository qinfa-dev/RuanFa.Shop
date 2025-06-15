using Coravel.Invocable;
using Microsoft.Extensions.Options;
using RuanFa.FashionShop.Application.Abstractions.Loggings.Services;
using RuanFa.FashionShop.Infrastructure.Settings;
using Serilog;

namespace RuanFa.FashionShop.Infrastructure.BackgroundJobs;

public class DeleteOldActivityLogsJob : IInvocable
{
    private readonly IActivityLogCleanupService _cleanupService;
    private readonly CleanupJobSettings _settings;

    public DeleteOldActivityLogsJob(IActivityLogCleanupService cleanupService, IOptions<BackgroundJobsSettings> options)
    {
        _cleanupService = cleanupService;
        _settings = options.Value.Cleanup;
    }

    public async Task Invoke()
    {
        await _cleanupService.DeleteOldLogsAsync(_settings.RetentionDays);

        var now = DateTimeOffset.UtcNow;

        // Build scheduled time for today in UTC using DateTimeOffset
        var scheduledToday = new DateTimeOffset(
            now.Year, now.Month, now.Day,
            _settings.ScheduleHour, _settings.ScheduleMinute, 0,
            TimeSpan.Zero); // zero offset means UTC

        DateTimeOffset nextRunUtc;

        if (now < scheduledToday)
        {
            nextRunUtc = scheduledToday;
        }
        else
        {
            nextRunUtc = scheduledToday.AddDays(1);
        }

        // Convert to local time zone offset
        var localZone = TimeZoneInfo.Local;
        var nextRunLocal = TimeZoneInfo.ConvertTime(nextRunUtc, localZone);

        Log.Information(
            "🕒 DeleteOldActivityLogsJob completed. Next scheduled run is at {NextRunUtc} UTC ({NextRunLocal} Local)",
            nextRunUtc.ToString("yyyy-MM-dd HH:mm"),
            nextRunLocal.ToString("yyyy-MM-dd HH:mm"));
    }
}
