using Microsoft.AspNetCore.SignalR;

namespace ServerCloudStore.API.Hubs;

/// <summary>
/// SignalR Hub para notificaciones de importación masiva
/// </summary>
public class ImportNotificationHub : Hub
{
    /// <summary>
    /// Se une a un grupo específico para recibir notificaciones de un job
    /// </summary>
    public async Task JoinJobGroup(string jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"job_{jobId}");
    }

    /// <summary>
    /// Abandona el grupo de un job
    /// </summary>
    public async Task LeaveJobGroup(string jobId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"job_{jobId}");
    }
}

