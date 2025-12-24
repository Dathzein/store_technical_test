using ServerCloudStore.Application.DTOs.BulkImport;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Interfaz para el servicio de notificaciones SignalR
/// </summary>
public interface INotificationService
{
    Task SendImportProgressAsync(Guid jobId, BulkImportStatusDto status);
}

