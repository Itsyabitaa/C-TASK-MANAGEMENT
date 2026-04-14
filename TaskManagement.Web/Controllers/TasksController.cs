using Microsoft.AspNetCore.Mvc;
using TaskManagement.Web.Models;
using TaskManagement.Web.Models.ViewModels;
using TaskManagement.Web.Services;

namespace TaskManagement.Web.Controllers;

public class TasksController : Controller
{
    private readonly ITaskService _tasks;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService tasks, ILogger<TasksController> logger)
    {
        _tasks = tasks;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] TaskItemStatus? status,
        CancellationToken cancellationToken)
    {
        var all = await _tasks.GetAllAsync(cancellationToken);
        var filtered = status.HasValue
            ? all.Where(t => t.Status == status.Value).ToList()
            : all;

        ViewBag.StatusFilter = status;
        return View(filtered);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TaskItemInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItemInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _tasks.CreateAsync(model, cancellationToken);
            TempData["Success"] = "Task created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create task failed");
            ModelState.AddModelError(string.Empty, "Could not save the task. Please try again.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await _tasks.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return NotFound();

        var model = ToInput(entity);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskItemInputModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var updated = await _tasks.UpdateAsync(id, model, cancellationToken);
            if (updated is null)
                return NotFound();

            TempData["Success"] = "Task updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update task failed");
            ModelState.AddModelError(string.Empty, "Could not update the task. Please try again.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var entity = await _tasks.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return NotFound();
        return View(entity);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _tasks.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        try
        {
            var ok = await _tasks.DeleteAsync(id, cancellationToken);
            if (!ok)
                return NotFound();

            TempData["Success"] = "Task deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete task failed");
            TempData["Error"] = "Could not delete the task.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    private static TaskItemInputModel ToInput(TaskItem entity)
    {
        return new TaskItemInputModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status,
            Priority = entity.Priority,
            DueDate = entity.DueDate
        };
    }

}
