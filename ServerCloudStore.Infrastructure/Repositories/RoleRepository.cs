using Dapper;
using Npgsql;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Infrastructure.Data;

namespace ServerCloudStore.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de roles usando Dapper
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Roles WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Id = id });
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        const string sql = "SELECT * FROM Roles WHERE Name = @Name";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Name = name });
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Roles ORDER BY Name";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        return await connection.QueryAsync<Role>(sql);
    }

    public async Task<int> CreateAsync(Role role)
    {
        const string sql = @"
            INSERT INTO Roles (Name, Permissions, CreatedAt)
            VALUES (@Name, @Permissions, @CreatedAt)
            RETURNING Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var id = await connection.QuerySingleAsync<int>(sql, new
        {
            role.Name,
            role.Permissions,
            role.CreatedAt
        });

        return id;
    }

    public async Task<bool> UpdateAsync(Role role)
    {
        const string sql = @"
            UPDATE Roles
            SET Name = @Name,
                Permissions = @Permissions
            WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            role.Id,
            role.Name,
            role.Permissions
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Roles WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }
}

