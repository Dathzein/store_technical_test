using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ServerCloudStore.Infrastructure.Data;

/// <summary>
/// Inicializador de base de datos que ejecuta los scripts SQL al iniciar la aplicación
/// </summary>
public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly string _scriptsPath;

    public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
        _scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
    }

    /// <summary>
    /// Inicializa la base de datos ejecutando los scripts SQL en orden
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando inicialización de base de datos...");

            // Verificar si las tablas ya existen
            if (await TablesExistAsync())
            {
                _logger.LogInformation("Las tablas ya existen. Saltando inicialización.");
                return;
            }

            // Ejecutar scripts en orden
            await ExecuteScriptAsync("001_CreateTables.sql");
            await ExecuteScriptAsync("002_CreateIndexes.sql");
            await ExecuteScriptAsync("003_CreateForeignKeys.sql");
            await ExecuteScriptAsync("004_SeedData.sql");

            _logger.LogInformation("Scripts SQL ejecutados correctamente.");
            
            // NOTA: El seeding de usuarios con contraseñas hasheadas se hará después
            // cuando se registren los repositorios en el contenedor de dependencias
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar la base de datos.");
            throw;
        }
    }

    /// <summary>
    /// Verifica si las tablas principales ya existen
    /// </summary>
    private async Task<bool> TablesExistAsync()
    {
        const string checkQuery = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'users'
            );";

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(checkQuery, connection);
        var result = await command.ExecuteScalarAsync();
        
        return result != null && (bool)result;
    }

    /// <summary>
    /// Ejecuta un script SQL desde el archivo
    /// </summary>
    private async Task ExecuteScriptAsync(string scriptFileName)
    {
        var scriptPath = Path.Combine(_scriptsPath, scriptFileName);
        
        if (!File.Exists(scriptPath))
        {
            // Intentar desde el directorio del proyecto
            var projectScriptsPath = Path.Combine(
                Directory.GetCurrentDirectory(), 
                "ServerCloudStore.Infrastructure", 
                "Scripts", 
                scriptFileName);
            
            if (File.Exists(projectScriptsPath))
            {
                scriptPath = projectScriptsPath;
            }
            else
            {
                _logger.LogWarning($"Script {scriptFileName} no encontrado. Saltando...");
                return;
            }
        }

        var scriptContent = await File.ReadAllTextAsync(scriptPath);
        
        if (string.IsNullOrWhiteSpace(scriptContent))
        {
            _logger.LogWarning($"Script {scriptFileName} está vacío. Saltando...");
            return;
        }

        _logger.LogInformation($"Ejecutando script: {scriptFileName}");

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        try
        {
            // Ejecutar todo el script de una vez
            await using var command = new NpgsqlCommand(scriptContent, connection);
            command.CommandTimeout = 120; // Timeout de 2 minutos
            await command.ExecuteNonQueryAsync();
            
            _logger.LogInformation($"Script {scriptFileName} ejecutado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al ejecutar script {scriptFileName}.");
            throw;
        }
    }
}

