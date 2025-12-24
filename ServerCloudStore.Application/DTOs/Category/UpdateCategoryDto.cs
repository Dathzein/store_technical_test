namespace ServerCloudStore.Application.DTOs.Category;

/// <summary>
/// DTO para actualizar una categoría
/// </summary>
public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

