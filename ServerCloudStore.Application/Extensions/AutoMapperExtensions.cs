using Microsoft.Extensions.DependencyInjection;
using ServerCloudStore.Application.Mappings;

namespace ServerCloudStore.Application.Extensions;

/// <summary>
/// Extensiones para configurar AutoMapper en la aplicación
/// </summary>
public static class AutoMapperExtensions
{
    /// <summary>
    /// Registra AutoMapper con los perfiles de mapeo configurados
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios para encadenamiento</returns>
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }
}

