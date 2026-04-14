namespace TaskManagement.Web.Models;

/// <summary>
/// Named TaskItemStatus to avoid clashing with System.Threading.Tasks.TaskStatus.
/// </summary>
public enum TaskItemStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2,
    Cancelled = 3
}
