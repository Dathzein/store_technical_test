namespace ServerCloudStore.Domain.Entities;

/// <summary>
/// Entidad que representa un trabajo de importación masiva
/// </summary>
public class BulkImportJob
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int FailedRecords { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

