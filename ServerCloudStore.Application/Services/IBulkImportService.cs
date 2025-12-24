using ServerCloudStore.Application.DTOs.BulkImport;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Interfaz para el servicio de importación masiva
/// </summary>
public interface IBulkImportService
{
    Task<Response<BulkImportJobDto>> StartImportAsync(BulkImportRequestDto request);
    Task<Response<BulkImportJobDto>> GetJobStatusAsync(Guid jobId);
    Task<Response<List<BulkImportJobDto>>> GetAllJobsAsync();
}

