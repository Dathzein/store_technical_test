using Microsoft.AspNetCore.Http;

namespace ServerCloudStore.API.DTOs;

/// <summary>
/// DTO de API para solicitar una importación masiva (usa IFormFile)
/// </summary>
public class BulkImportRequestApiDto
{
    /// <summary>
    /// Si se proporciona, se generarán esta cantidad de productos aleatorios
    /// </summary>
    public int? GenerateCount { get; set; }
    
    /// <summary>
    /// Archivo CSV a procesar
    /// </summary>
    public IFormFile? CsvFile { get; set; }
}

