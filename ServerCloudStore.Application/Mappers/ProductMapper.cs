using ServerCloudStore.Application.DTOs.Product;
using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Application.Mappers;

/// <summary>
/// Mapper para convertir entre entidades Product y DTOs
/// </summary>
public static class ProductMapper
{
    /// <summary>
    /// Mapea CreateProductDto a entidad Product
    /// </summary>
    public static Product ToDomain(CreateProductDto dto)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Mapea UpdateProductDto a entidad Product (solo campos actualizables)
    /// </summary>
    public static void UpdateDomain(Product product, UpdateProductDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mapea entidad Product a ProductDto (con información de categoría)
    /// </summary>
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            CategoryImageUrl = product.Category?.ImageUrl,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    /// <summary>
    /// Mapea entidad Product a ProductListDto (versión simplificada)
    /// </summary>
    public static ProductListDto ToListDto(Product product)
    {
        return new ProductListDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name
        };
    }

    /// <summary>
    /// Mapea una lista de entidades Product a lista de ProductListDto
    /// </summary>
    public static List<ProductListDto> ToListDtoList(IEnumerable<Product> products)
    {
        return products.Select(ToListDto).ToList();
    }
}

