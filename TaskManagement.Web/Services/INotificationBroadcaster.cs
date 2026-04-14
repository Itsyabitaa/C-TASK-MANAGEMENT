namespace TaskManagement.Web.Services;

public interface INotificationBroadcaster
{
    Task BroadcastAsync(string message, string category = "info", CancellationToken cancellationToken = default);
}
