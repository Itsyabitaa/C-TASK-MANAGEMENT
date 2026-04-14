using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Web.Models.ViewModels;

public class TaskItemInputModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters.")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000, ErrorMessage = "Description cannot exceed 4000 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Status")]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    [Required]
    [Display(Name = "Priority")]
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;

    [Display(Name = "Due date")]
    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }
}
