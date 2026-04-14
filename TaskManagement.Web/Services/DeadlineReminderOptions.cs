namespace TaskManagement.Web.Services;

public class DeadlineReminderOptions
{
    public const string SectionName = "DeadlineReminder";

    /// <summary>How often the background service checks for upcoming deadlines.</summary>
    public int CheckIntervalMinutes { get; set; } = 1;

    /// <summary>Notify when a task is due within this many hours (and not completed/cancelled).</summary>
    public int HoursBeforeDue { get; set; } = 24;
}
