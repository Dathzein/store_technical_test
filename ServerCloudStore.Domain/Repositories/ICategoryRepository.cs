using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Domain.Repositories;

/// <summary>
/// Interfaz para el repositorio de categorías
/// </summary>
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<int> CreateAsync(Category category);
    Task<bool> UpdateAsync(Category category);
    Task<bool> DeleteAsync(int id);
}

