namespace ServerCloudStore.Application.DTOs.Product;

/// <summary>
/// DTO para actualizar un producto
/// </summary>
public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
}

