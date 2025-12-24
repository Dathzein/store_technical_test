using Dapper;
using Npgsql;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Infrastructure.Data;

namespace ServerCloudStore.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de categorías usando Dapper
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Categories WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Categories ORDER BY Name";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        return await connection.QueryAsync<Category>(sql);
    }

    public async Task<int> CreateAsync(Category category)
    {
        const string sql = @"
            INSERT INTO Categories (Name, Description, ImageUrl, CreatedAt, UpdatedAt)
            VALUES (@Name, @Description, @ImageUrl, @CreatedAt, @UpdatedAt)
            RETURNING Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var id = await connection.QuerySingleAsync<int>(sql, new
        {
            category.Name,
            category.Description,
            category.ImageUrl,
            category.CreatedAt,
            category.UpdatedAt
        });

        return id;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        const string sql = @"
            UPDATE Categories
            SET Name = @Name,
                Description = @Description,
                ImageUrl = @ImageUrl,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            category.Id,
            category.Name,
            category.Description,
            category.ImageUrl,
            category.UpdatedAt
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Categories WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }
}

