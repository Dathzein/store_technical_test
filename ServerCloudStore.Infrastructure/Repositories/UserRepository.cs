using System.Data;
using Dapper;
using Npgsql;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Infrastructure.Data;

namespace ServerCloudStore.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de usuarios usando Dapper
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT u.*, r.Id, r.Name, r.Permissions, r.CreatedAt
            FROM Users u
            INNER JOIN Roles r ON u.RoleId = r.Id
            WHERE u.Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var result = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Id = id },
            splitOn: "Id");

        return result.FirstOrDefault();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        const string sql = @"
            SELECT u.*, r.Id, r.Name, r.Permissions, r.CreatedAt
            FROM Users u
            INNER JOIN Roles r ON u.RoleId = r.Id
            WHERE u.Username = @Username";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var result = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Username = username },
            splitOn: "Id");

        return result.FirstOrDefault();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = @"
            SELECT u.*, r.Id, r.Name, r.Permissions, r.CreatedAt
            FROM Users u
            INNER JOIN Roles r ON u.RoleId = r.Id
            WHERE u.Email = @Email";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var result = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Email = email },
            splitOn: "Id");

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = @"
            SELECT u.*, r.Id, r.Name, r.Permissions, r.CreatedAt
            FROM Users u
            INNER JOIN Roles r ON u.RoleId = r.Id
            ORDER BY u.Username";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var result = await connection.QueryAsync<User, Role, User>(
            sql,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            splitOn: "Id");

        return result;
    }

    public async Task<int> CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO Users (Username, Email, PasswordHash, RoleId, CreatedAt, UpdatedAt)
            VALUES (@Username, @Email, @PasswordHash, @RoleId, @CreatedAt, @UpdatedAt)
            RETURNING Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var id = await connection.QuerySingleAsync<int>(sql, new
        {
            user.Username,
            user.Email,
            user.PasswordHash,
            user.RoleId,
            user.CreatedAt,
            user.UpdatedAt
        });

        return id;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE Users
            SET Username = @Username,
                Email = @Email,
                PasswordHash = @PasswordHash,
                RoleId = @RoleId,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            user.Id,
            user.Username,
            user.Email,
            user.PasswordHash,
            user.RoleId,
            user.UpdatedAt
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Users WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }
}

