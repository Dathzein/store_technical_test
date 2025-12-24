namespace ServerCloudStore.Application.DTOs.BulkImport;

/// <summary>
/// DTO para representar un trabajo de importación masiva
/// </summary>
public class BulkImportJobDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int FailedRecords { get; set; }
    public int SuccessRecords => ProcessedRecords - FailedRecords;
    public double ProgressPercentage => TotalRecords > 0 ? (double)ProcessedRecords / TotalRecords * 100 : 0;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

