using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace ServerCloudStore.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos para gestionar la conexión a PostgreSQL
/// </summary>
public class ApplicationDbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    /// <summary>
    /// Obtiene una conexión a la base de datos
    /// </summary>
    public IDbConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}

