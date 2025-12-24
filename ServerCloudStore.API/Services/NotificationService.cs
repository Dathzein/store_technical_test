using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ServerCloudStore.API.Hubs;
using ServerCloudStore.Application.DTOs.BulkImport;
using ServerCloudStore.Application.Services;

namespace ServerCloudStore.API.Services;

/// <summary>
/// Servicio para enviar notificaciones SignalR
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IHubContext<ImportNotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<ImportNotificationHub> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendImportProgressAsync(Guid jobId, BulkImportStatusDto status)
    {
        try
        {
            await _hubContext.Clients.Group($"job_{jobId}").SendAsync("ImportProgress", status);
            _logger.LogDebug("Notificación enviada para job {JobId}: {Status} - {Processed}/{Total}", 
                jobId, status.Status, status.ProcessedRecords, status.TotalRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación SignalR para job {JobId}", jobId);
        }
    }
}

