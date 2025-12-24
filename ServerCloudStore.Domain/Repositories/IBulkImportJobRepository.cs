using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Domain.Repositories;

/// <summary>
/// Interfaz para el repositorio de trabajos de importación masiva
/// </summary>
public interface IBulkImportJobRepository
{
    Task<BulkImportJob?> GetByIdAsync(Guid id);
    Task<IEnumerable<BulkImportJob>> GetAllAsync();
    Task<Guid> CreateAsync(BulkImportJob job);
    Task<bool> UpdateAsync(BulkImportJob job);
    Task<bool> DeleteAsync(Guid id);
}

