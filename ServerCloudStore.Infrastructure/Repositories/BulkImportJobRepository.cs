using Dapper;
using Npgsql;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Infrastructure.Data;

namespace ServerCloudStore.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de trabajos de importación masiva usando Dapper
/// </summary>
public class BulkImportJobRepository : IBulkImportJobRepository
{
    private readonly ApplicationDbContext _context;

    public BulkImportJobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BulkImportJob?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM BulkImportJobs WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        return await connection.QueryFirstOrDefaultAsync<BulkImportJob>(sql, new { Id = id });
    }

    public async Task<IEnumerable<BulkImportJob>> GetAllAsync()
    {
        const string sql = "SELECT * FROM BulkImportJobs ORDER BY CreatedAt DESC";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        return await connection.QueryAsync<BulkImportJob>(sql);
    }

    public async Task<Guid> CreateAsync(BulkImportJob job)
    {
        const string sql = @"
            INSERT INTO BulkImportJobs (Id, Status, TotalRecords, ProcessedRecords, FailedRecords, StartedAt, CompletedAt, ErrorMessage, CreatedAt)
            VALUES (@Id, @Status, @TotalRecords, @ProcessedRecords, @FailedRecords, @StartedAt, @CompletedAt, @ErrorMessage, @CreatedAt)
            RETURNING Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var id = await connection.QuerySingleAsync<Guid>(sql, new
        {
            job.Id,
            job.Status,
            job.TotalRecords,
            job.ProcessedRecords,
            job.FailedRecords,
            job.StartedAt,
            job.CompletedAt,
            job.ErrorMessage,
            job.CreatedAt
        });

        return id;
    }

    public async Task<bool> UpdateAsync(BulkImportJob job)
    {
        const string sql = @"
            UPDATE BulkImportJobs
            SET Status = @Status,
                TotalRecords = @TotalRecords,
                ProcessedRecords = @ProcessedRecords,
                FailedRecords = @FailedRecords,
                StartedAt = @StartedAt,
                CompletedAt = @CompletedAt,
                ErrorMessage = @ErrorMessage
            WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            job.Id,
            job.Status,
            job.TotalRecords,
            job.ProcessedRecords,
            job.FailedRecords,
            job.StartedAt,
            job.CompletedAt,
            job.ErrorMessage
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM BulkImportJobs WHERE Id = @Id";

        await using var connection = (NpgsqlConnection)_context.GetConnection();
        await connection.OpenAsync();

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }
}

