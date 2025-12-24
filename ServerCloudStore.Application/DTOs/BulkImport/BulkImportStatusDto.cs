namespace ServerCloudStore.Application.DTOs.BulkImport;

/// <summary>
/// DTO para notificaciones de estado de importación (SignalR)
/// </summary>
public class BulkImportStatusDto
{
    public Guid JobId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int FailedRecords { get; set; }
    public double ProgressPercentage { get; set; }
    public string? ErrorMessage { get; set; }
}

