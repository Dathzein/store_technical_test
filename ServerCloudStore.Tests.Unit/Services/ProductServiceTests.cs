using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ServerCloudStore.Application.DTOs.Common;
using ServerCloudStore.Application.DTOs.Product;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;

namespace ServerCloudStore.Tests.Unit.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _mockMapper = new Mock<IMapper>();

        _productService = new ProductService(
            _mockProductRepository.Object,
            _mockCategoryRepository.Object,
            _mockLogger.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidProduct_ReturnsSuccessResponse()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        var category = new Category { Id = 1, Name = "Test Category" };
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            Stock = 10,
            CategoryId = 1,
            Category = category
        };
        var productDto = new ProductDto { Id = 1, Name = "Test Product" };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(createDto.CategoryId))
            .ReturnsAsync(category);

        _mockMapper
            .Setup(m => m.Map<Product>(createDto))
            .Returns(new Product { Name = createDto.Name, CategoryId = createDto.CategoryId });

        _mockProductRepository
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(1);

        _mockMapper
            .Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(productDto);

        // Act
        var result = await _productService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Message.Should().Contain("creado exitosamente");
    }

    [Fact]
    public async Task CreateAsync_InvalidCategory_ReturnsErrorResponse()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Test Product",
            CategoryId = 999
        };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(createDto.CategoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _productService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(400);
        result.Message.Should().Contain("categoría");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsSuccessResponse()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Name = "Test Product" };
        var productDto = new ProductDto { Id = productId, Name = "Test Product" };

        _mockProductRepository
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockMapper
            .Setup(m => m.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(productId);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingProduct_ReturnsNotFoundResponse()
    {
        // Arrange
        var productId = 999;

        _mockProductRepository
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
        result.Message.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        // Arrange
        var query = new ProductQueryDto { Page = 1, PageSize = 10 };
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1" },
            new Product { Id = 2, Name = "Product 2" }
        };
        var productListDtos = new List<ProductListDto>
        {
            new ProductListDto { Id = 1, Name = "Product 1" },
            new ProductListDto { Id = 2, Name = "Product 2" }
        };

        _mockProductRepository
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<int?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((products, 2));

        _mockMapper
            .Setup(m => m.Map<List<ProductListDto>>(products))
            .Returns(productListDtos);

        // Act
        var result = await _productService.GetAllAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
        result.Data.Page.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ValidProduct_ReturnsSuccessResponse()
    {
        // Arrange
        var productId = 1;
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            CategoryId = 1
        };
        var existingProduct = new Product { Id = productId, Name = "Old Name" };
        var category = new Category { Id = 1, Name = "Test Category" };
        var productDto = new ProductDto { Id = productId, Name = "Updated Product" };

        _mockProductRepository
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(updateDto.CategoryId))
            .ReturnsAsync(category);

        _mockMapper
            .Setup(m => m.Map(updateDto, existingProduct))
            .Returns(existingProduct);

        _mockProductRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        _mockMapper
            .Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(productDto);

        // Act
        var result = await _productService.UpdateAsync(productId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Message.Should().Contain("actualizado exitosamente");
    }

    [Fact]
    public async Task UpdateAsync_NonExistingProduct_ReturnsNotFoundResponse()
    {
        // Arrange
        var productId = 999;
        var updateDto = new UpdateProductDto { Name = "Updated", CategoryId = 1 };

        _mockProductRepository
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.UpdateAsync(productId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
    }

    [Fact]
    public async Task DeleteAsync_ExistingProduct_ReturnsSuccessResponse()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Name = "Test Product" };

        _mockProductRepository
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockProductRepository
            .Setup(r => r.DeleteAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Message.Should().Contain("eliminado exitosamente");
    }

    [Fact]
    public async Task DeleteAsync_NonExistingProduct_ReturnsNotFoundResponse()
    {
        // Arrange
        var productId = 999;

        _mockProductRepository
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
    }
}

