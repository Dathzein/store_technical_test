namespace ServerCloudStore.Domain.Services;

/// <summary>
/// Interfaz para procesar archivos CSV
/// </summary>
public interface ICsvProcessor
{
    Task<List<ProductData>> ParseCsvAsync(Stream csvStream);
}

/// <summary>
/// Clase auxiliar para datos de producto desde CSV
/// </summary>
public class ProductData
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
}

