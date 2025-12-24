using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Services;

namespace ServerCloudStore.Infrastructure.Services;

/// <summary>
/// Servicio para generar datos de prueba
/// </summary>
public class DataGeneratorService : IDataGenerator
{
    private readonly Random _random = new();
    private readonly string[] _productNames = new[]
    {
        "Servidor Dell PowerEdge", "Servidor HP ProLiant", "Servidor IBM System x",
        "Servidor Lenovo ThinkSystem", "Servidor Supermicro", "Servidor Cisco UCS",
        "Servidor Fujitsu Primergy", "Servidor HPE Apollo", "Servidor Dell VxRail",
        "Servidor Nutanix", "VMware vSphere", "Microsoft Hyper-V",
        "AWS EC2 Instance", "Azure Virtual Machine", "Google Cloud Compute",
        "Docker Container", "Kubernetes Cluster", "OpenStack Cloud",
        "Red Hat OpenShift", "VMware Cloud Foundation"
    };

    private readonly string[] _descriptions = new[]
    {
        "Servidor de alto rendimiento para aplicaciones empresariales",
        "Solución cloud escalable y confiable",
        "Infraestructura virtualizada optimizada",
        "Plataforma de cómputo de próxima generación",
        "Sistema de almacenamiento y procesamiento avanzado"
    };

    /// <summary>
    /// Genera una lista de productos aleatorios
    /// </summary>
    public List<Product> GenerateProducts(int count, List<int> categoryIds)
    {
        if (!categoryIds.Any())
        {
            throw new InvalidOperationException("Se requiere al menos una categoría para generar productos");
        }

        var products = new List<Product>();
        var usedNames = new HashSet<string>();

        for (int i = 0; i < count; i++)
        {
            var product = new Product
            {
                Name = GenerateUniqueProductName(usedNames),
                Description = _descriptions[_random.Next(_descriptions.Length)],
                Price = Math.Round((decimal)(_random.NextDouble() * 99000 + 1000), 2), // Entre $1,000 y $100,000
                Stock = _random.Next(0, 1000),
                CategoryId = categoryIds[_random.Next(categoryIds.Count)],
                CreatedAt = DateTime.UtcNow
            };

            products.Add(product);
        }

        return products;
    }

    private string GenerateUniqueProductName(HashSet<string> usedNames)
    {
        string name;
        int attempts = 0;
        
        do
        {
            var baseName = _productNames[_random.Next(_productNames.Length)];
            var variant = _random.Next(1000, 9999);
            name = $"{baseName} {variant}";
            attempts++;
        } while (usedNames.Contains(name) && attempts < 100);

        usedNames.Add(name);
        return name;
    }
}

