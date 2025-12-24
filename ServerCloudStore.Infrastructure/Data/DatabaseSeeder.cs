using Microsoft.Extensions.Logging;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Domain.Services;

namespace ServerCloudStore.Infrastructure.Data;

/// <summary>
/// Seeder para poblar la base de datos con datos iniciales
/// </summary>
public class DatabaseSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICategoryRepository categoryRepository,
        IPasswordHasher passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _categoryRepository = categoryRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el seeding de datos iniciales
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando seeding de datos...");

            // Verificar si ya existen usuarios
            var existingUsers = await _userRepository.GetAllAsync();
            if (existingUsers.Any())
            {
                _logger.LogInformation("Los datos ya existen. Saltando seeding.");
                return;
            }

            // Obtener roles
            var adminRole = await _roleRepository.GetByNameAsync("Admin");
            var userRole = await _roleRepository.GetByNameAsync("User");

            if (adminRole == null || userRole == null)
            {
                _logger.LogWarning("Los roles no existen. Asegúrate de ejecutar los scripts SQL primero.");
                return;
            }

            // Crear usuarios con contraseñas hasheadas
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@servercloudstore.com",
                PasswordHash = _passwordHasher.HashPassword("Admin123!"),
                RoleId = adminRole.Id,
                CreatedAt = DateTime.UtcNow
            };

            var regularUser = new User
            {
                Username = "user",
                Email = "user@servercloudstore.com",
                PasswordHash = _passwordHasher.HashPassword("User123!"),
                RoleId = userRole.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(adminUser);
            await _userRepository.CreateAsync(regularUser);

            _logger.LogInformation("Seeding de usuarios completado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar seeding de datos.");
            throw;
        }
    }
}

