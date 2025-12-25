using System.Text.Json;
using AutoMapper;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Application.DTOs.BulkImport;
using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Application.DTOs.Product;
using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper que centraliza todos los mapeos del sistema
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigureAuthMappings();
        ConfigureCategoryMappings();
        ConfigureProductMappings();
        ConfigureBulkImportMappings();
    }

    /// <summary>
    /// Configuración de mapeos para autenticación
    /// </summary>
    private void ConfigureAuthMappings()
    {
        // User -> UserDto (con lógica custom para deserializar permisos JSON)
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleName, 
                opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty))
            .ForMember(dest => dest.Permissions, 
                opt => opt.MapFrom(src => DeserializePermissions(src.Role)));
    }

    /// <summary>
    /// Configuración de mapeos para categorías
    /// </summary>
    private void ConfigureCategoryMappings()
    {
        // Category <-> CategoryDto (bidireccional)
        CreateMap<Category, CategoryDto>().ReverseMap();

        // CreateCategoryDto -> Category
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());

        // UpdateCategoryDto -> Category (para actualizar entidad existente)
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Products, opt => opt.Ignore());
    }

    /// <summary>
    /// Configuración de mapeos para productos
    /// </summary>
    private void ConfigureProductMappings()
    {
        // Product -> ProductDto
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, 
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.CategoryImageUrl, 
                opt => opt.MapFrom(src => src.Category != null ? src.Category.ImageUrl : null));

        // Product -> ProductListDto
        CreateMap<Product, ProductListDto>()
            .ForMember(dest => dest.CategoryName, 
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        // CreateProductDto -> Product
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        // UpdateProductDto -> Product (para actualizar entidad existente)
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Category, opt => opt.Ignore());
    }

    /// <summary>
    /// Configuración de mapeos para importación masiva
    /// </summary>
    private void ConfigureBulkImportMappings()
    {
        // BulkImportJob -> BulkImportJobDto
        CreateMap<BulkImportJob, BulkImportJobDto>();

        // BulkImportJob -> BulkImportStatusDto (con cálculo de progreso)
        CreateMap<BulkImportJob, BulkImportStatusDto>()
            .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProgressPercentage, 
                opt => opt.MapFrom(src => src.TotalRecords > 0 
                    ? (double)src.ProcessedRecords / src.TotalRecords * 100 
                    : 0));
    }

    /// <summary>
    /// Método auxiliar para deserializar los permisos desde JSON
    /// </summary>
    private static List<string> DeserializePermissions(Role? role)
    {
        if (role == null || string.IsNullOrEmpty(role.Permissions))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(role.Permissions) ?? new List<string>();
        }
        catch
        {
            // Si no se puede parsear, retornar lista vacía
            return new List<string>();
        }
    }
}

