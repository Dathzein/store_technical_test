using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.API.Controllers;

/// <summary>
/// Controller para gestión de categorías
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva categoría
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(Response<CategoryDto>), 201)]
    [ProducesResponseType(typeof(Response<CategoryDto>), 400)]
    [ProducesResponseType(typeof(Response<CategoryDto>), 401)]
    public async Task<ActionResult<Response<CategoryDto>>> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(Response<CategoryDto>.Error("Datos de entrada inválidos", 400));
        }

        var result = await _categoryService.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return StatusCode(201, result);
    }

    /// <summary>
    /// Obtiene todas las categorías
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Response<List<CategoryDto>>), 200)]
    public async Task<ActionResult<Response<List<CategoryDto>>>> GetAll()
    {
        var result = await _categoryService.GetAllAsync();

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene una categoría por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Response<CategoryDto>), 200)]
    [ProducesResponseType(typeof(Response<CategoryDto>), 404)]
    public async Task<ActionResult<Response<CategoryDto>>> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Actualiza una categoría
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(Response<CategoryDto>), 200)]
    [ProducesResponseType(typeof(Response<CategoryDto>), 404)]
    [ProducesResponseType(typeof(Response<CategoryDto>), 401)]
    public async Task<ActionResult<Response<CategoryDto>>> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(Response<CategoryDto>.Error("Datos de entrada inválidos", 400));
        }

        var result = await _categoryService.UpdateAsync(id, dto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Elimina una categoría
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(Response<bool>), 200)]
    [ProducesResponseType(typeof(Response<bool>), 404)]
    [ProducesResponseType(typeof(Response<bool>), 401)]
    public async Task<ActionResult<Response<bool>>> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }
}

