namespace ServerCloudStore.Application.DTOs.Product;

/// <summary>
/// DTO simplificado para listado de productos
/// </summary>
public class ProductListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

