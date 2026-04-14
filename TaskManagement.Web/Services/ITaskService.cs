using TaskManagement.Web.Models;
using TaskManagement.Web.Models.ViewModels;

namespace TaskManagement.Web.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateAsync(TaskItemInputModel input, CancellationToken cancellationToken = default);
    Task<TaskItem?> UpdateAsync(int id, TaskItemInputModel input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<DashboardStats> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
    Task<ReportSummary> GetReportSummaryAsync(CancellationToken cancellationToken = default);
}

public sealed class DashboardStats
{
    public int Total { get; init; }
    public int Todo { get; init; }
    public int InProgress { get; init; }
    public int Done { get; init; }
    public int Overdue { get; init; }
    public int DueThisWeek { get; init; }
}

public sealed class ReportSummary
{
    public IReadOnlyList<StatusCountRow> CountsByStatus { get; init; } = Array.Empty<StatusCountRow>();
    public int OverdueCount { get; init; }
    public IReadOnlyList<TaskItem> OverdueTasks { get; init; } = Array.Empty<TaskItem>();
    public IReadOnlyList<TaskItem> DueThisWeekTasks { get; init; } = Array.Empty<TaskItem>();
}

public sealed class StatusCountRow
{
    public TaskStatus Status { get; init; }
    public int Count { get; init; }
}
