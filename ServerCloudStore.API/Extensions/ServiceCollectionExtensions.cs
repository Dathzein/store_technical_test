using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using ServerCloudStore.Application.Extensions;
using ServerCloudStore.Infrastructure.Extensions;

namespace ServerCloudStore.API.Extensions;

/// <summary>
/// Extension methods for service collection configuration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura la autenticación JWT
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey no configurada");
        var issuer = configuration["JwtSettings:Issuer"] ?? "ServerCloudStoreAPI";
        var audience = configuration["JwtSettings:Audience"] ?? "ServerCloudStoreClient";

        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    /// <summary>
    /// Configura las políticas de autorización
    /// </summary>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Política para requerir rol Admin
            options.AddPolicy("RequireAdminRole", policy =>
                policy.RequireRole("Admin"));

            // Política para requerir rol User
            options.AddPolicy("RequireUserRole", policy =>
                policy.RequireRole("User", "Admin"));

            // Política para requerir permiso de escritura de productos
            options.AddPolicy("RequireProductWritePermission", policy =>
                policy.RequireClaim("Permission", "products:write"));

            // Política para requerir permiso de lectura de productos
            options.AddPolicy("RequireProductReadPermission", policy =>
                policy.RequireClaim("Permission", "products:read", "products:write"));
        });

        return services;
    }

    /// <summary>
    /// Registra todos los servicios de la aplicación
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registrar AutoMapper
        services.AddAutoMapperConfiguration();

        // Registrar servicios de Infrastructure
        services.AddInfrastructure(configuration);

        // Registrar servicios de Application
        services.AddApplication();

        // Registrar servicios de API (NotificationService)
        services.AddScoped<Application.Services.INotificationService, Services.NotificationService>();

        return services;
    }
}
