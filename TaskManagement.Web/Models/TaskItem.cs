using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Web.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    public TaskPriority Priority { get; set; } = TaskPriority.Normal;

    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// When we last pushed a SignalR reminder for this task's deadline (avoids spam).
    /// </summary>
    public DateTime? LastDeadlineNotifiedAt { get; set; }
}
