using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskManagement.Web.Data;
using TaskManagement.Web.Models;

namespace TaskManagement.Web.Services;

public class DeadlineNotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IOptions<DeadlineReminderOptions> _options;
    private readonly ILogger<DeadlineNotificationBackgroundService> _logger;

    public DeadlineNotificationBackgroundService(
        IServiceProvider services,
        IOptions<DeadlineReminderOptions> options,
        ILogger<DeadlineNotificationBackgroundService> logger)
    {
        _services = services;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(Math.Max(1, _options.Value.CheckIntervalMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunReminderPassAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Deadline reminder pass failed");
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async Task RunReminderPassAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var broadcaster = scope.ServiceProvider.GetRequiredService<INotificationBroadcaster>();
        var time = scope.ServiceProvider.GetRequiredService<TimeProvider>();
        var now = time.GetUtcNow().UtcDateTime;
        var hours = Math.Max(1, _options.Value.HoursBeforeDue);
        var horizon = now.AddHours(hours);

        var pending = await db.TaskItems
            .Where(t =>
                t.DueDate.HasValue
                && t.Status != TaskItemStatus.Done
                && t.Status != TaskItemStatus.Cancelled
                && t.DueDate > now
                && t.DueDate <= horizon)
            .ToListAsync(cancellationToken);

        foreach (var task in pending)
        {
            if (task.LastDeadlineNotifiedAt.HasValue)
                continue;

            var due = task.DueDate!.Value;
            var message =
                $"Deadline reminder: \"{task.Title}\" is due {due:yyyy-MM-dd HH:mm} UTC ({hours}h window).";

            await broadcaster.BroadcastAsync(message, "warning", cancellationToken);

            task.LastDeadlineNotifiedAt = now;
        }

        var overdue = await db.TaskItems
            .Where(t =>
                t.DueDate.HasValue
                && t.DueDate < now
                && t.Status != TaskItemStatus.Done
                && t.Status != TaskItemStatus.Cancelled)
            .ToListAsync(cancellationToken);

        foreach (var task in overdue)
        {
            var last = task.LastDeadlineNotifiedAt;
            if (last.HasValue && (now - last.Value).TotalHours < 24)
                continue;

            var message = $"Overdue: \"{task.Title}\" was due {task.DueDate:yyyy-MM-dd HH:mm} UTC.";
            await broadcaster.BroadcastAsync(message, "danger", cancellationToken);
            task.LastDeadlineNotifiedAt = now;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
