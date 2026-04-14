using Microsoft.EntityFrameworkCore;
using TaskManagement.Web.Models;

namespace TaskManagement.Web.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.TaskItems.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;

        db.TaskItems.AddRange(
            new TaskItem
            {
                Title = "Review course requirements",
                Description = "Confirm rubric items for the C# project.",
                Status = TaskItemStatus.Done,
                Priority = TaskPriority.Normal,
                DueDate = now.AddDays(-2),
                CreatedAt = now.AddDays(-10),
                UpdatedAt = now.AddDays(-2)
            },
            new TaskItem
            {
                Title = "Implement CRUD and validation",
                Status = TaskItemStatus.InProgress,
                Priority = TaskPriority.High,
                DueDate = now.AddDays(3),
                CreatedAt = now.AddDays(-5),
                UpdatedAt = now.AddDays(-1)
            },
            new TaskItem
            {
                Title = "Add SignalR notifications",
                Status = TaskItemStatus.Todo,
                Priority = TaskPriority.Urgent,
                DueDate = now.AddHours(20),
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1)
            },
            new TaskItem
            {
                Title = "Write README and push to GitHub",
                Status = TaskItemStatus.Todo,
                Priority = TaskPriority.Low,
                DueDate = now.AddDays(7),
                CreatedAt = now,
                UpdatedAt = now
            });

        await db.SaveChangesAsync(cancellationToken);
    }
}
