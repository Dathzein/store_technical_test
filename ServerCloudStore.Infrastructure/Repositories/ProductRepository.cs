using Dapper;
using Npgsql;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Infrastructure.Data;

namespace ServerCloudStore.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de productos usando Dapper
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT p.*, c.Id, c.Name, c.Description, c.ImageUrl, c.CreatedAt, c.UpdatedAt
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var result = await connection.QueryAsync<Product, Category, Product>(
            sql,
            (product, category) =>
            {
                product.Category = category;
                return product;
            },
            new { Id = id },
            splitOn: "Id");

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        const string sql = @"
            SELECT p.*, c.Id, c.Name, c.Description, c.ImageUrl, c.CreatedAt, c.UpdatedAt
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            ORDER BY p.Name";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var result = await connection.QueryAsync<Product, Category, Product>(
            sql,
            (product, category) =>
            {
                product.Category = category;
                return product;
            },
            splitOn: "Id");

        return result;
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
    {
        const string sql = @"
            SELECT p.*, c.Id, c.Name, c.Description, c.ImageUrl, c.CreatedAt, c.UpdatedAt
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.CategoryId = @CategoryId
            ORDER BY p.Name";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var result = await connection.QueryAsync<Product, Category, Product>(
            sql,
            (product, category) =>
            {
                product.Category = category;
                return product;
            },
            new { CategoryId = categoryId },
            splitOn: "Id");

        return result;
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int? minStock = null,
        string? sortBy = null,
        string? sortOrder = null)
    {
        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        // Construir WHERE clause dinámicamente
        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            whereConditions.Add("(p.Name ILIKE @Search OR p.Description ILIKE @Search)");
            parameters.Add("Search", $"%{search}%");
        }

        if (categoryId.HasValue)
        {
            whereConditions.Add("p.CategoryId = @CategoryId");
            parameters.Add("CategoryId", categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            whereConditions.Add("p.Price >= @MinPrice");
            parameters.Add("MinPrice", minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            whereConditions.Add("p.Price <= @MaxPrice");
            parameters.Add("MaxPrice", maxPrice.Value);
        }

        if (minStock.HasValue)
        {
            whereConditions.Add("p.Stock >= @MinStock");
            parameters.Add("MinStock", minStock.Value);
        }

        var whereClause = whereConditions.Any() 
            ? "WHERE " + string.Join(" AND ", whereConditions)
            : string.Empty;

        // Construir ORDER BY
        var validSortBy = new[] { "name", "price", "createdat" };
        var sortField = validSortBy.Contains(sortBy?.ToLower()) ? sortBy!.ToLower() : "name";
        var sortDirection = sortOrder?.ToLower() == "desc" ? "DESC" : "ASC";

        var orderByClause = sortField switch
        {
            "price" => $"ORDER BY p.Price {sortDirection}",
            "createdat" => $"ORDER BY p.CreatedAt {sortDirection}",
            _ => $"ORDER BY p.Name {sortDirection}"
        };

        // Query para contar total
        var countSql = $@"
            SELECT COUNT(*)
            FROM Products p
            {whereClause}";

        var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

        // Query para obtener productos paginados
        var offset = (page - 1) * pageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", pageSize);

        var productsSql = $@"
            SELECT p.*, c.Id, c.Name, c.Description, c.ImageUrl, c.CreatedAt, c.UpdatedAt
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            {whereClause}
            {orderByClause}
            LIMIT @PageSize OFFSET @Offset";

        var products = await connection.QueryAsync<Product, Category, Product>(
            productsSql,
            (product, category) =>
            {
                product.Category = category;
                return product;
            },
            parameters,
            splitOn: "Id");

        return (products, totalCount);
    }

    public async Task<int> CreateAsync(Product product)
    {
        const string sql = @"
            INSERT INTO Products (Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
            VALUES (@Name, @Description, @Price, @Stock, @CategoryId, @CreatedAt, @UpdatedAt)
            RETURNING Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var id = await connection.QuerySingleAsync<int>(sql, new
        {
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.CreatedAt,
            product.UpdatedAt
        });

        return id;
    }

    public async Task<bool> BulkCreateAsync(IEnumerable<Product> products)
    {
        const string sql = @"
            INSERT INTO Products (Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt)
            VALUES (@Name, @Description, @Price, @Stock, @CategoryId, @CreatedAt, @UpdatedAt)";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, products.Select(p => new
        {
            p.Name,
            p.Description,
            p.Price,
            p.Stock,
            p.CategoryId,
            p.CreatedAt,
            p.UpdatedAt
        }));

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        const string sql = @"
            UPDATE Products
            SET Name = @Name,
                Description = @Description,
                Price = @Price,
                Stock = @Stock,
                CategoryId = @CategoryId,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.UpdatedAt
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Products WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }
}

