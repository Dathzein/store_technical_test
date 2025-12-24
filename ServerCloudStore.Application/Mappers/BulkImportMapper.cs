using ServerCloudStore.Application.DTOs.BulkImport;
using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Application.Mappers;

/// <summary>
/// Mapper para convertir entre entidades BulkImportJob y DTOs
/// </summary>
public static class BulkImportMapper
{
    /// <summary>
    /// Mapea entidad BulkImportJob a BulkImportJobDto
    /// </summary>
    public static BulkImportJobDto ToDto(BulkImportJob job)
    {
        return new BulkImportJobDto
        {
            Id = job.Id,
            Status = job.Status,
            TotalRecords = job.TotalRecords,
            ProcessedRecords = job.ProcessedRecords,
            FailedRecords = job.FailedRecords,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
            ErrorMessage = job.ErrorMessage,
            CreatedAt = job.CreatedAt
        };
    }

    /// <summary>
    /// Mapea entidad BulkImportJob a BulkImportStatusDto (para SignalR)
    /// </summary>
    public static BulkImportStatusDto ToStatusDto(BulkImportJob job)
    {
        var progressPercentage = job.TotalRecords > 0 
            ? (double)job.ProcessedRecords / job.TotalRecords * 100 
            : 0;

        return new BulkImportStatusDto
        {
            JobId = job.Id,
            Status = job.Status,
            TotalRecords = job.TotalRecords,
            ProcessedRecords = job.ProcessedRecords,
            FailedRecords = job.FailedRecords,
            ProgressPercentage = progressPercentage,
            ErrorMessage = job.ErrorMessage
        };
    }
}

