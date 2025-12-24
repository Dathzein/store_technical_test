namespace ServerCloudStore.Application.DTOs.BulkImport;

/// <summary>
/// DTO para solicitar una importación masiva
/// </summary>
public class BulkImportRequestDto
{
    /// <summary>
    /// Si se proporciona, se generarán esta cantidad de productos aleatorios
    /// </summary>
    public int? GenerateCount { get; set; }
    
    /// <summary>
    /// Stream del archivo CSV a procesar
    /// </summary>
    public Stream? CsvStream { get; set; }
    
    /// <summary>
    /// Nombre del archivo CSV (opcional, para logging)
    /// </summary>
    public string? CsvFileName { get; set; }
}

