using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerCloudStore.API.DTOs;
using ServerCloudStore.Application.DTOs.BulkImport;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.API.Controllers;

/// <summary>
/// Controller para importación masiva de productos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdminRole")]
public class BulkImportController : ControllerBase
{
    private readonly IBulkImportService _bulkImportService;
    private readonly ILogger<BulkImportController> _logger;

    public BulkImportController(IBulkImportService bulkImportService, ILogger<BulkImportController> logger)
    {
        _bulkImportService = bulkImportService;
        _logger = logger;
    }

    /// <summary>
    /// Inicia una importación masiva de productos desde CSV o genera productos aleatorios
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Response<BulkImportJobDto>), 202)]
    [ProducesResponseType(typeof(Response<BulkImportJobDto>), 400)]
    [ProducesResponseType(typeof(Response<BulkImportJobDto>), 401)]
    public async Task<ActionResult<Response<BulkImportJobDto>>> StartImport([FromForm] BulkImportRequestApiDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(Response<BulkImportJobDto>.Error("Datos de entrada inválidos", 400));
        }

        // Validar que se proporcione al menos uno de los dos
        if (!request.GenerateCount.HasValue && request.CsvFile == null)
        {
            return BadRequest(Response<BulkImportJobDto>.Error("Debe proporcionar GenerateCount o CsvFile", 400));
        }

        // Convertir DTO de API a DTO de Application
        var applicationDto = new BulkImportRequestDto
        {
            GenerateCount = request.GenerateCount
        };

        if (request.CsvFile != null)
        {
            // Copiar el stream a un MemoryStream para evitar que se cierre cuando termine la petición HTTP
            var memoryStream = new MemoryStream();
            await request.CsvFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Resetear posición para lectura
            
            applicationDto.CsvStream = memoryStream;
            applicationDto.CsvFileName = request.CsvFile.FileName;
        }

        var result = await _bulkImportService.StartImportAsync(applicationDto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return StatusCode(202, result);
    }

    /// <summary>
    /// Obtiene el estado de un trabajo de importación
    /// </summary>
    [HttpGet("{jobId}")]
    [ProducesResponseType(typeof(Response<BulkImportJobDto>), 200)]
    [ProducesResponseType(typeof(Response<BulkImportJobDto>), 404)]
    public async Task<ActionResult<Response<BulkImportJobDto>>> GetJobStatus(Guid jobId)
    {
        var result = await _bulkImportService.GetJobStatusAsync(jobId);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene todos los trabajos de importación
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Response<List<BulkImportJobDto>>), 200)]
    public async Task<ActionResult<Response<List<BulkImportJobDto>>>> GetAllJobs()
    {
        var result = await _bulkImportService.GetAllJobsAsync();

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }
}

