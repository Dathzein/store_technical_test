using Microsoft.Extensions.Logging;
using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Application.Mappers;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Servicio de aplicación para categorías
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Response<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        try
        {
            var category = CategoryMapper.ToDomain(dto);
            var id = await _categoryRepository.CreateAsync(category);
            category.Id = id;

            var result = CategoryMapper.ToDto(category);
            return Response<CategoryDto>.Success(result, "Categoría creada exitosamente", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría: {Name}", dto.Name);
            return Response<CategoryDto>.Error("Error al crear la categoría", 500);
        }
    }

    public async Task<Response<List<CategoryDto>>> GetAllAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            var result = CategoryMapper.ToDtoList(categories);
            return Response<List<CategoryDto>>.Success(result, "Categorías obtenidas exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías");
            return Response<List<CategoryDto>>.Error("Error al obtener las categorías", 500);
        }
    }

    public async Task<Response<CategoryDto>> GetByIdAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
            {
                return Response<CategoryDto>.Error("Categoría no encontrada", 404);
            }

            var result = CategoryMapper.ToDto(category);
            return Response<CategoryDto>.Success(result, "Categoría obtenida exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categoría: {Id}", id);
            return Response<CategoryDto>.Error("Error al obtener la categoría", 500);
        }
    }

    public async Task<Response<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
            {
                return Response<CategoryDto>.Error("Categoría no encontrada", 404);
            }

            CategoryMapper.UpdateDomain(category, dto);
            var updated = await _categoryRepository.UpdateAsync(category);

            if (!updated)
            {
                return Response<CategoryDto>.Error("Error al actualizar la categoría", 500);
            }

            var result = CategoryMapper.ToDto(category);
            return Response<CategoryDto>.Success(result, "Categoría actualizada exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar categoría: {Id}", id);
            return Response<CategoryDto>.Error("Error al actualizar la categoría", 500);
        }
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
            {
                return Response<bool>.Error("Categoría no encontrada", 404);
            }

            var deleted = await _categoryRepository.DeleteAsync(id);

            if (!deleted)
            {
                return Response<bool>.Error("Error al eliminar la categoría", 500);
            }

            return Response<bool>.Success(true, "Categoría eliminada exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar categoría: {Id}", id);
            return Response<bool>.Error("Error al eliminar la categoría", 500);
        }
    }
}

