using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Domain.Services;

/// <summary>
/// Interfaz para generar datos de prueba
/// </summary>
public interface IDataGenerator
{
    List<Product> GenerateProducts(int count, List<int> categoryIds);
}

