using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Domain.Services;
using ServerCloudStore.Infrastructure.Data;

namespace ServerCloudStore.Infrastructure.Extensions;

/// <summary>
/// Extensiones para registrar servicios de Infrastructure
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra los servicios de Infrastructure
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Registrar DbContext
        services.AddScoped<ApplicationDbContext>();

        // Registrar DatabaseInitializer
        services.AddScoped<DatabaseInitializer>();

        // Registrar DatabaseSeeder
        services.AddScoped<DatabaseSeeder>();

        // Registrar repositorios
        services.AddScoped<IUserRepository, Repositories.UserRepository>();
        services.AddScoped<IRoleRepository, Repositories.RoleRepository>();
        services.AddScoped<ICategoryRepository, Repositories.CategoryRepository>();
        services.AddScoped<IProductRepository, Repositories.ProductRepository>();
        services.AddScoped<IBulkImportJobRepository, Repositories.BulkImportJobRepository>();

        // Registrar servicios de Infrastructure
        services.AddScoped<Domain.Services.ICsvProcessor, Services.CsvProcessorService>();
        services.AddScoped<Domain.Services.IDataGenerator, Services.DataGeneratorService>();

        return services;
    }
}

