namespace ServerCloudStore.Application.DTOs.Product;

/// <summary>
/// DTO para consultas de productos con filtros y paginación
/// </summary>
public class ProductQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinStock { get; set; }
    public string? SortBy { get; set; } // name, price, createdAt
    public string? SortOrder { get; set; } // asc, desc
}

