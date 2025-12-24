using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using ServerCloudStore.Domain.Services;

namespace ServerCloudStore.Infrastructure.Services;

/// <summary>
/// Servicio para procesar archivos CSV
/// </summary>
public class CsvProcessorService : ICsvProcessor
{
    private readonly ILogger<CsvProcessorService> _logger;

    public CsvProcessorService(ILogger<CsvProcessorService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Parsea un archivo CSV y retorna una lista de productos
    /// </summary>
    public async Task<List<ProductData>> ParseCsvAsync(Stream csvStream)
    {
        var products = new List<ProductData>();
        var errors = new List<string>();

        using var reader = new StreamReader(csvStream, Encoding.UTF8);
        
        // Leer encabezados
        var headerLine = await reader.ReadLineAsync();
        if (headerLine == null)
        {
            throw new InvalidOperationException("El archivo CSV está vacío");
        }

        var headers = ParseCsvLine(headerLine);
        var expectedHeaders = new[] { "Name", "Description", "Price", "Stock", "CategoryId" };
        
        // Validar encabezados
        if (!expectedHeaders.All(h => headers.Contains(h, StringComparer.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"El CSV debe contener las columnas: {string.Join(", ", expectedHeaders)}");
        }

        int lineNumber = 1;
        string? line;
        
        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                var values = ParseCsvLine(line);
                
                if (values.Count < expectedHeaders.Length)
                {
                    errors.Add($"Línea {lineNumber}: Número insuficiente de columnas");
                    continue;
                }

                var nameIndex = Array.FindIndex(headers.ToArray(), h => h.Equals("Name", StringComparison.OrdinalIgnoreCase));
                var descriptionIndex = Array.FindIndex(headers.ToArray(), h => h.Equals("Description", StringComparison.OrdinalIgnoreCase));
                var priceIndex = Array.FindIndex(headers.ToArray(), h => h.Equals("Price", StringComparison.OrdinalIgnoreCase));
                var stockIndex = Array.FindIndex(headers.ToArray(), h => h.Equals("Stock", StringComparison.OrdinalIgnoreCase));
                var categoryIdIndex = Array.FindIndex(headers.ToArray(), h => h.Equals("CategoryId", StringComparison.OrdinalIgnoreCase));

                var product = new ProductData
                {
                    Name = values[nameIndex]?.Trim() ?? string.Empty,
                    Description = values[descriptionIndex]?.Trim(),
                    Price = decimal.Parse(values[priceIndex]?.Trim() ?? "0", CultureInfo.InvariantCulture),
                    Stock = int.Parse(values[stockIndex]?.Trim() ?? "0", CultureInfo.InvariantCulture),
                    CategoryId = int.Parse(values[categoryIdIndex]?.Trim() ?? "0", CultureInfo.InvariantCulture)
                };

                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    errors.Add($"Línea {lineNumber}: El nombre es requerido");
                    continue;
                }

                if (product.Price <= 0)
                {
                    errors.Add($"Línea {lineNumber}: El precio debe ser mayor a 0");
                    continue;
                }

                if (product.Stock < 0)
                {
                    errors.Add($"Línea {lineNumber}: El stock no puede ser negativo");
                    continue;
                }

                if (product.CategoryId <= 0)
                {
                    errors.Add($"Línea {lineNumber}: CategoryId debe ser mayor a 0");
                    continue;
                }

                products.Add(product);
            }
            catch (Exception ex)
            {
                errors.Add($"Línea {lineNumber}: {ex.Message}");
                _logger.LogWarning(ex, "Error al procesar línea {LineNumber} del CSV", lineNumber);
            }
        }

        if (errors.Any())
        {
            _logger.LogWarning("Errores encontrados al procesar CSV: {Errors}", string.Join("; ", errors));
        }

        return products;
    }

    /// <summary>
    /// Parsea una línea CSV manejando comillas y valores separados por comas
    /// </summary>
    private List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var currentValue = new StringBuilder();
        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Comilla escapada
                    currentValue.Append('"');
                    i++; // Saltar la siguiente comilla
                }
                else
                {
                    // Toggle inside quotes
                    insideQuotes = !insideQuotes;
                }
            }
            else if (c == ',' && !insideQuotes)
            {
                values.Add(currentValue.ToString());
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(c);
            }
        }

        // Agregar el último valor
        values.Add(currentValue.ToString());

        return values;
    }
}

