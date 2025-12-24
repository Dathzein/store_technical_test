using ServerCloudStore.Application.DTOs.Common;
using ServerCloudStore.Application.DTOs.Product;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Application.Services;

/// <summary>
/// Interfaz para el servicio de productos
/// </summary>
public interface IProductService
{
    Task<Response<ProductDto>> CreateAsync(CreateProductDto dto);
    Task<Response<PagedResultDto<ProductListDto>>> GetAllAsync(ProductQueryDto query);
    Task<Response<ProductDto>> GetByIdAsync(int id);
    Task<Response<ProductDto>> UpdateAsync(int id, UpdateProductDto dto);
    Task<Response<bool>> DeleteAsync(int id);
}

