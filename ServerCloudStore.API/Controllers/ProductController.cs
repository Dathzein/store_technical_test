using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerCloudStore.Application.DTOs.Common;
using ServerCloudStore.Application.DTOs.Product;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.API.Controllers;

/// <summary>
/// Controller para gestión de productos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireProductWritePermission")]
    [ProducesResponseType(typeof(Response<ProductDto>), 201)]
    [ProducesResponseType(typeof(Response<ProductDto>), 400)]
    [ProducesResponseType(typeof(Response<ProductDto>), 401)]
    public async Task<ActionResult<Response<ProductDto>>> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(Response<ProductDto>.Error("Datos de entrada inválidos", 400));
        }

        var result = await _productService.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return StatusCode(201, result);
    }

    /// <summary>
    /// Obtiene productos con paginación, filtros y búsqueda
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Response<PagedResultDto<ProductListDto>>), 200)]
    public async Task<ActionResult<Response<PagedResultDto<ProductListDto>>>> GetAll([FromQuery] ProductQueryDto query)
    {
        var result = await _productService.GetAllAsync(query);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene un producto por ID con información de la categoría
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Response<ProductDto>), 200)]
    [ProducesResponseType(typeof(Response<ProductDto>), 404)]
    public async Task<ActionResult<Response<ProductDto>>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Actualiza un producto
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequireProductWritePermission")]
    [ProducesResponseType(typeof(Response<ProductDto>), 200)]
    [ProducesResponseType(typeof(Response<ProductDto>), 404)]
    [ProducesResponseType(typeof(Response<ProductDto>), 401)]
    public async Task<ActionResult<Response<ProductDto>>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(Response<ProductDto>.Error("Datos de entrada inválidos", 400));
        }

        var result = await _productService.UpdateAsync(id, dto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Elimina un producto
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireProductWritePermission")]
    [ProducesResponseType(typeof(Response<bool>), 200)]
    [ProducesResponseType(typeof(Response<bool>), 404)]
    [ProducesResponseType(typeof(Response<bool>), 401)]
    public async Task<ActionResult<Response<bool>>> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Code, result);
        }

        return Ok(result);
    }
}

