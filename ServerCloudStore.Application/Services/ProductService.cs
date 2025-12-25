using AutoMapper;
using Microsoft.Extensions.Logging;
using ServerCloudStore.Application.DTOs.Common;
using ServerCloudStore.Application.DTOs.Product;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Servicio de aplicación para productos
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ILogger<ProductService> logger,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Response<ProductDto>> CreateAsync(CreateProductDto dto)
    {
        try
        {
            // Validar que la categoría existe
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return Response<ProductDto>.Error("La categoría especificada no existe", 400);
            }

            var product = _mapper.Map<Product>(dto);
            var id = await _productRepository.CreateAsync(product);
            product.Id = id;

            // Cargar categoría para el DTO
            product.Category = category;

            var result = _mapper.Map<ProductDto>(product);
            return Response<ProductDto>.Success(result, "Producto creado exitosamente", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto: {Name}", dto.Name);
            return Response<ProductDto>.Error("Error al crear el producto", 500);
        }
    }

    public async Task<Response<PagedResultDto<ProductListDto>>> GetAllAsync(ProductQueryDto query)
    {
        try
        {
            // Validar y normalizar parámetros
            var page = Math.Max(1, query.Page);
            var pageSize = Math.Min(100, Math.Max(1, query.PageSize)); // Máximo 100 por página
            var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? "name" : query.SortBy.ToLower();
            var sortOrder = string.IsNullOrWhiteSpace(query.SortOrder) ? "asc" : query.SortOrder.ToLower();

            var (products, totalCount) = await _productRepository.GetPagedAsync(
                page,
                pageSize,
                query.Search,
                query.CategoryId,
                query.MinPrice,
                query.MaxPrice,
                query.MinStock,
                sortBy,
                sortOrder);

            var items = _mapper.Map<List<ProductListDto>>(products);

            var result = new PagedResultDto<ProductListDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Response<PagedResultDto<ProductListDto>>.Success(result, "Productos obtenidos exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            return Response<PagedResultDto<ProductListDto>>.Error("Error al obtener los productos", 500);
        }
    }

    public async Task<Response<ProductDto>> GetByIdAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                return Response<ProductDto>.Error("Producto no encontrado", 404);
            }

            var result = _mapper.Map<ProductDto>(product);
            return Response<ProductDto>.Success(result, "Producto obtenido exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto: {Id}", id);
            return Response<ProductDto>.Error("Error al obtener el producto", 500);
        }
    }

    public async Task<Response<ProductDto>> UpdateAsync(int id, UpdateProductDto dto)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                return Response<ProductDto>.Error("Producto no encontrado", 404);
            }

            // Validar que la categoría existe
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return Response<ProductDto>.Error("La categoría especificada no existe", 400);
            }

            _mapper.Map(dto, product);
            var updated = await _productRepository.UpdateAsync(product);

            if (!updated)
            {
                return Response<ProductDto>.Error("Error al actualizar el producto", 500);
            }

            product.Category = category;
            var result = _mapper.Map<ProductDto>(product);
            return Response<ProductDto>.Success(result, "Producto actualizado exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto: {Id}", id);
            return Response<ProductDto>.Error("Error al actualizar el producto", 500);
        }
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                return Response<bool>.Error("Producto no encontrado", 404);
            }

            var deleted = await _productRepository.DeleteAsync(id);

            if (!deleted)
            {
                return Response<bool>.Error("Error al eliminar el producto", 500);
            }

            return Response<bool>.Success(true, "Producto eliminado exitosamente", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto: {Id}", id);
            return Response<bool>.Error("Error al eliminar el producto", 500);
        }
    }
}

