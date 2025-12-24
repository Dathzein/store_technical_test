using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Domain.Repositories;

/// <summary>
/// Interfaz para el repositorio de productos
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null, 
        int? categoryId = null, 
        decimal? minPrice = null, 
        decimal? maxPrice = null,
        int? minStock = null,
        string? sortBy = null,
        string? sortOrder = null);
    Task<int> CreateAsync(Product product);
    Task<bool> BulkCreateAsync(IEnumerable<Product> products);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}

