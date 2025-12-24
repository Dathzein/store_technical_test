using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Application.Validators;
using ServerCloudStore.Domain.Services;

namespace ServerCloudStore.Application.Extensions;

/// <summary>
/// Extensiones para registrar servicios de Application
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra los servicios de Application
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        // Registrar servicios de Application
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBulkImportService, BulkImportService>();

        // Registrar servicios de Domain
        services.AddScoped<IPasswordHasher, Domain.Services.PasswordHasher>();
        services.AddScoped<IAuthDomainService, Domain.Services.AuthDomainService>();

        return services;
    }
}

