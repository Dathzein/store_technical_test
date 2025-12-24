using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Interfaz para el servicio de categorías
/// </summary>
public interface ICategoryService
{
    Task<Response<CategoryDto>> CreateAsync(CreateCategoryDto dto);
    Task<Response<List<CategoryDto>>> GetAllAsync();
    Task<Response<CategoryDto>> GetByIdAsync(int id);
    Task<Response<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<Response<bool>> DeleteAsync(int id);
}

