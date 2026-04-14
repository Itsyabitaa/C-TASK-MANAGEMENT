using Microsoft.AspNetCore.SignalR;
using TaskManagement.Web.Hubs;

namespace TaskManagement.Web.Services;

public class NotificationBroadcaster : INotificationBroadcaster
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationBroadcaster> _logger;

    public NotificationBroadcaster(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationBroadcaster> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task BroadcastAsync(string message, string category = "info", CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(
                "ReceiveNotification",
                message,
                category,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to broadcast notification");
        }
    }
}
