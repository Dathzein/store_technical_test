using Microsoft.Extensions.Logging;
using ServerCloudStore.Application.DTOs.BulkImport;
using ServerCloudStore.Application.Mappers;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Domain.Services;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Servicio de aplicación para importación masiva
/// </summary>
public class BulkImportService : IBulkImportService
{
    private readonly IBulkImportJobRepository _jobRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly INotificationService _notificationService;
    private readonly ICsvProcessor _csvProcessor;
    private readonly IDataGenerator _dataGenerator;
    private readonly ILogger<BulkImportService> _logger;

    public BulkImportService(
        IBulkImportJobRepository jobRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        INotificationService notificationService,
        ICsvProcessor csvProcessor,
        IDataGenerator dataGenerator,
        ILogger<BulkImportService> logger)
    {
        _jobRepository = jobRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _notificationService = notificationService;
        _csvProcessor = csvProcessor;
        _dataGenerator = dataGenerator;
        _logger = logger;
    }

    public async Task<Response<BulkImportJobDto>> StartImportAsync(BulkImportRequestDto request)
    {
        try
        {
            var job = new BulkImportJob
            {
                Id = Guid.NewGuid(),
                Status = "Pending",
                TotalRecords = 0,
                ProcessedRecords = 0,
                FailedRecords = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _jobRepository.CreateAsync(job);

            // Iniciar procesamiento en background (fire and forget)
            _ = Task.Run(async () => await ProcessImportAsync(job, request));

            var result = BulkImportMapper.ToDto(job);
            return Response<BulkImportJobDto>.Success(result, "Importación iniciada", 202);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar importación masiva");
            return Response<BulkImportJobDto>.Error("Error al iniciar la importación", 500);
        }
    }

    public async Task<Response<BulkImportJobDto>> GetJobStatusAsync(Guid jobId)
    {
        try
        {
            var job = await _jobRepository.GetByIdAsync(jobId);

            if (job == null)
            {
                return Response<BulkImportJobDto>.Error("Trabajo de importación no encontrado", 404);
            }

            var result = BulkImportMapper.ToDto(job);
            return Response<BulkImportJobDto>.Success(result, "Estado obtenido exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estado del job: {JobId}", jobId);
            return Response<BulkImportJobDto>.Error("Error al obtener el estado", 500);
        }
    }

    public async Task<Response<List<BulkImportJobDto>>> GetAllJobsAsync()
    {
        try
        {
            var jobs = await _jobRepository.GetAllAsync();
            var result = jobs.Select(BulkImportMapper.ToDto).ToList();
            return Response<List<BulkImportJobDto>>.Success(result, "Trabajos obtenidos exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener trabajos de importación");
            return Response<List<BulkImportJobDto>>.Error("Error al obtener los trabajos", 500);
        }
    }

    /// <summary>
    /// Procesa la importación de forma asíncrona
    /// </summary>
    private async Task ProcessImportAsync(BulkImportJob job, BulkImportRequestDto request)
    {
        try
        {
            job.Status = "Processing";
            job.StartedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job);

            await NotifyProgressAsync(job);

            List<Product> products;

            // Obtener productos según el tipo de importación
            if (request.CsvStream != null)
            {
                var productData = await _csvProcessor.ParseCsvAsync(request.CsvStream);
                products = productData.Select(p => new Product
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }
            else if (request.GenerateCount.HasValue)
            {
                var allCategories = await _categoryRepository.GetAllAsync();
                var categoryIds = allCategories.Select(c => c.Id).ToList();

                if (!categoryIds.Any())
                {
                    throw new InvalidOperationException("No hay categorías disponibles para generar productos");
                }

                var generatedProducts = _dataGenerator.GenerateProducts(request.GenerateCount.Value, categoryIds);
                products = generatedProducts;
            }
            else
            {
                throw new InvalidOperationException("Debe proporcionar CsvStream o GenerateCount");
            }

            job.TotalRecords = products.Count;
            await _jobRepository.UpdateAsync(job);
            await NotifyProgressAsync(job);

            // Validar categorías antes de procesar
            var validCategories = await _categoryRepository.GetAllAsync();
            var validCategoryIds = validCategories.Select(c => c.Id).ToHashSet();

            // Procesar en lotes para optimizar
            const int batchSize = 1000;
            var batches = products.Chunk(batchSize);
            int processedCount = 0;
            int failedCount = 0;

            foreach (var batch in batches)
            {
                var validProducts = new List<Product>();

                foreach (var product in batch)
                {
                    if (!validCategoryIds.Contains(product.CategoryId))
                    {
                        failedCount++;
                        _logger.LogWarning("Categoría inválida para producto: {ProductName}, CategoryId: {CategoryId}", 
                            product.Name, product.CategoryId);
                        continue;
                    }

                    validProducts.Add(product);
                }

                if (validProducts.Any())
                {
                    try
                    {
                        await _productRepository.BulkCreateAsync(validProducts);
                        processedCount += validProducts.Count;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al insertar lote de productos");
                        failedCount += validProducts.Count;
                    }
                }

                job.ProcessedRecords = processedCount + failedCount;
                job.FailedRecords = failedCount;
                await _jobRepository.UpdateAsync(job);
                await NotifyProgressAsync(job);
            }

            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;
            job.ProcessedRecords = processedCount + failedCount;
            job.FailedRecords = failedCount;
            await _jobRepository.UpdateAsync(job);
            await NotifyProgressAsync(job);

            _logger.LogInformation("Importación completada: Job {JobId}, Procesados: {Processed}, Fallidos: {Failed}", 
                job.Id, processedCount, failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar importación: Job {JobId}", job.Id);
            job.Status = "Failed";
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorMessage = ex.Message;
            await _jobRepository.UpdateAsync(job);
            await NotifyProgressAsync(job);
        }
    }

    /// <summary>
    /// Envía notificación de progreso vía SignalR
    /// </summary>
    private async Task NotifyProgressAsync(BulkImportJob job)
    {
        var statusDto = BulkImportMapper.ToStatusDto(job);
        await _notificationService.SendImportProgressAsync(job.Id, statusDto);
    }
}

