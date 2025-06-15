namespace RuanFa.FashionShop.Infrastructure.Settings;

public class BackgroundJobsSettings
{
    public const string Section = "BackgroundJobsSettings";

    public CleanupJobSettings Cleanup { get; set; } = new();
    public bool EnableCleanupJob => Cleanup.Enabled;
}

public class CleanupJobSettings
{
    public bool Enabled { get; set; } = true;
    public int RetentionDays { get; set; } = 30;
    public int ScheduleHour { get; set; } = 2;
    public int ScheduleMinute { get; set; } = 0;
}
