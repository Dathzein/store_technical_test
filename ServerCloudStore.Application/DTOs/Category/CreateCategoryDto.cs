namespace ServerCloudStore.Application.DTOs.Category;

/// <summary>
/// DTO para crear una categoría
/// </summary>
public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

