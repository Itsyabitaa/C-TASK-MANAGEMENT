using Microsoft.AspNetCore.Mvc;
using TaskManagement.Web.Services;

namespace TaskManagement.Web.Controllers;

public class ReportsController : Controller
{
    private readonly ITaskService _tasks;

    public ReportsController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var summary = await _tasks.GetReportSummaryAsync(cancellationToken);
        return View(summary);
    }
}
