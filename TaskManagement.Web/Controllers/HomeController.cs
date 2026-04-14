using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Web.Models;
using TaskManagement.Web.Services;

namespace TaskManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly ITaskService _tasks;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ITaskService tasks, ILogger<HomeController> logger)
    {
        _tasks = tasks;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var stats = await _tasks.GetDashboardStatsAsync(cancellationToken);
        return View(stats);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var message = feature?.Error?.Message;
        return View(new ErrorViewModel { RequestId = requestId, Message = message });
    }
}
