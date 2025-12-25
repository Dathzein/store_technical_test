using AutoMapper;
using Microsoft.Extensions.Logging;
using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Domain.Entities;
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
    private readonly IMapper _mapper;

    public CategoryService(
        ICategoryRepository categoryRepository, 
        ILogger<CategoryService> logger,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Response<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        try
        {
            var category = _mapper.Map<Category>(dto);
            var id = await _categoryRepository.CreateAsync(category);
            category.Id = id;

            var result = _mapper.Map<CategoryDto>(category);
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
            var result = _mapper.Map<List<CategoryDto>>(categories);
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

            var result = _mapper.Map<CategoryDto>(category);
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

            _mapper.Map(dto, category);
            var updated = await _categoryRepository.UpdateAsync(category);

            if (!updated)
            {
                return Response<CategoryDto>.Error("Error al actualizar la categoría", 500);
            }

            var result = _mapper.Map<CategoryDto>(category);
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

