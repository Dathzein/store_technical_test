using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Application.Mappers;

/// <summary>
/// Mapper para convertir entre entidades Category y DTOs
/// </summary>
public static class CategoryMapper
{
    /// <summary>
    /// Mapea CreateCategoryDto a entidad Category
    /// </summary>
    public static Category ToDomain(CreateCategoryDto dto)
    {
        return new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Mapea UpdateCategoryDto a entidad Category (solo campos actualizables)
    /// </summary>
    public static void UpdateDomain(Category category, UpdateCategoryDto dto)
    {
        category.Name = dto.Name;
        category.Description = dto.Description;
        category.ImageUrl = dto.ImageUrl;
        category.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mapea entidad Category a CategoryDto
    /// </summary>
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    /// <summary>
    /// Mapea una lista de entidades Category a lista de CategoryDto
    /// </summary>
    public static List<CategoryDto> ToDtoList(IEnumerable<Category> categories)
    {
        return categories.Select(ToDto).ToList();
    }
}

