using Microsoft.EntityFrameworkCore;
using TaskManagement.Web.Data;
using TaskManagement.Web.Models;
using TaskManagement.Web.Models.ViewModels;

namespace TaskManagement.Web.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;
    private readonly INotificationBroadcaster _notifications;
    private readonly TimeProvider _time;

    public TaskService(AppDbContext db, INotificationBroadcaster notifications, TimeProvider time)
    {
        _db = db;
        _notifications = notifications;
        _time = time;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.TaskItems
            .AsNoTracking()
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.TaskItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<TaskItem> CreateAsync(TaskItemInputModel input, CancellationToken cancellationToken = default)
    {
        var now = _time.GetUtcNow().UtcDateTime;
        var entity = new TaskItem
        {
            Title = input.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
            Status = input.Status,
            Priority = input.Priority,
            DueDate = input.DueDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.TaskItems.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        await _notifications.BroadcastAsync(
            $"New task created: \"{entity.Title}\"",
            "success",
            cancellationToken);

        return entity;
    }

    public async Task<TaskItem?> UpdateAsync(int id, TaskItemInputModel input, CancellationToken cancellationToken = default)
    {
        var entity = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is null)
            return null;

        var previousDue = entity.DueDate;
        entity.Title = input.Title.Trim();
        entity.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
        entity.Status = input.Status;
        entity.Priority = input.Priority;
        entity.DueDate = input.DueDate;
        if (previousDue != entity.DueDate)
            entity.LastDeadlineNotifiedAt = null;
        entity.UpdatedAt = _time.GetUtcNow().UtcDateTime;

        await _db.SaveChangesAsync(cancellationToken);

        await _notifications.BroadcastAsync(
            $"Task updated: \"{entity.Title}\" (status: {entity.Status})",
            "info",
            cancellationToken);

        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is null)
            return false;

        var title = entity.Title;
        _db.TaskItems.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        await _notifications.BroadcastAsync(
            $"Task deleted: \"{title}\"",
            "warning",
            cancellationToken);

        return true;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var now = _time.GetUtcNow();
        var startOfWeek = StartOfWeekUtc(now);
        var endOfWeek = startOfWeek.AddDays(7);

        var items = await _db.TaskItems.AsNoTracking().ToListAsync(cancellationToken);

        var overdue = items.Count(t =>
            t.DueDate.HasValue
            && t.Status is not (TaskItemStatus.Done or TaskItemStatus.Cancelled)
            && t.DueDate.Value < now.UtcDateTime);

        var dueThisWeek = items.Count(t =>
            t.DueDate.HasValue
            && t.Status is not (TaskItemStatus.Done or TaskItemStatus.Cancelled)
            && t.DueDate.Value >= startOfWeek
            && t.DueDate.Value < endOfWeek);

        return new DashboardStats
        {
            Total = items.Count,
            Todo = items.Count(t => t.Status == TaskItemStatus.Todo),
            InProgress = items.Count(t => t.Status == TaskItemStatus.InProgress),
            Done = items.Count(t => t.Status == TaskItemStatus.Done),
            Overdue = overdue,
            DueThisWeek = dueThisWeek
        };
    }

    public async Task<ReportSummary> GetReportSummaryAsync(CancellationToken cancellationToken = default)
    {
        var now = _time.GetUtcNow();
        var startOfWeek = StartOfWeekUtc(now);
        var endOfWeek = startOfWeek.AddDays(7);

        var items = await _db.TaskItems.AsNoTracking().ToListAsync(cancellationToken);

        var counts = Enum.GetValues<TaskItemStatus>()
            .Select(s => new StatusCountRow { Status = s, Count = items.Count(t => t.Status == s) })
            .ToList();

        var overdueTasks = items
            .Where(t =>
                t.DueDate.HasValue
                && t.Status is not (TaskItemStatus.Done or TaskItemStatus.Cancelled)
                && t.DueDate.Value < now.UtcDateTime)
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToList();

        var dueWeek = items
            .Where(t =>
                t.DueDate.HasValue
                && t.Status is not (TaskItemStatus.Done or TaskItemStatus.Cancelled)
                && t.DueDate.Value >= startOfWeek
                && t.DueDate.Value < endOfWeek)
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToList();

        return new ReportSummary
        {
            CountsByStatus = counts,
            OverdueCount = overdueTasks.Count,
            OverdueTasks = overdueTasks,
            DueThisWeekTasks = dueWeek
        };
    }

    private static DateTime StartOfWeekUtc(DateTimeOffset now)
    {
        var utc = now.UtcDateTime.Date;
        var diff = (int)utc.DayOfWeek - (int)DayOfWeek.Monday;
        if (diff < 0) diff += 7;
        return utc.AddDays(-diff);
    }
}
