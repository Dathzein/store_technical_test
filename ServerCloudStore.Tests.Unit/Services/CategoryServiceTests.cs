using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;

namespace ServerCloudStore.Tests.Unit.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<ILogger<CategoryService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockLogger = new Mock<ILogger<CategoryService>>();
        _mockMapper = new Mock<IMapper>();

        _categoryService = new CategoryService(
            _mockCategoryRepository.Object,
            _mockLogger.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidCategory_ReturnsSuccessResponse()
    {
        // Arrange
        var createDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test Description"
        };
        var category = new Category { Id = 1, Name = "Test Category" };
        var categoryDto = new CategoryDto { Id = 1, Name = "Test Category" };

        _mockMapper
            .Setup(m => m.Map<Category>(createDto))
            .Returns(new Category { Name = createDto.Name });

        _mockCategoryRepository
            .Setup(r => r.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(1);

        _mockMapper
            .Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns(categoryDto);

        // Act
        var result = await _categoryService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Message.Should().Contain("creada exitosamente");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Category 1" },
            new Category { Id = 2, Name = "Category 2" }
        };
        var categoryDtos = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Category 1" },
            new CategoryDto { Id = 2, Name = "Category 2" }
        };

        _mockCategoryRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(categories);

        _mockMapper
            .Setup(m => m.Map<List<CategoryDto>>(categories))
            .Returns(categoryDtos);

        // Act
        var result = await _categoryService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCategory_ReturnsSuccessResponse()
    {
        // Arrange
        var categoryId = 1;
        var category = new Category { Id = categoryId, Name = "Test Category" };
        var categoryDto = new CategoryDto { Id = categoryId, Name = "Test Category" };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mockMapper
            .Setup(m => m.Map<CategoryDto>(category))
            .Returns(categoryDto);

        // Act
        var result = await _categoryService.GetByIdAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingCategory_ReturnsNotFoundResponse()
    {
        // Arrange
        var categoryId = 999;

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.GetByIdAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
        result.Message.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task UpdateAsync_ValidCategory_ReturnsSuccessResponse()
    {
        // Arrange
        var categoryId = 1;
        var updateDto = new UpdateCategoryDto { Name = "Updated Category" };
        var existingCategory = new Category { Id = categoryId, Name = "Old Name" };
        var categoryDto = new CategoryDto { Id = categoryId, Name = "Updated Category" };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(existingCategory);

        _mockMapper
            .Setup(m => m.Map(updateDto, existingCategory))
            .Returns(existingCategory);

        _mockCategoryRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(true);

        _mockMapper
            .Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns(categoryDto);

        // Act
        var result = await _categoryService.UpdateAsync(categoryId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Message.Should().Contain("actualizada exitosamente");
    }

    [Fact]
    public async Task UpdateAsync_NonExistingCategory_ReturnsNotFoundResponse()
    {
        // Arrange
        var categoryId = 999;
        var updateDto = new UpdateCategoryDto { Name = "Updated" };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.UpdateAsync(categoryId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
    }

    [Fact]
    public async Task DeleteAsync_ExistingCategory_ReturnsSuccessResponse()
    {
        // Arrange
        var categoryId = 1;
        var category = new Category { Id = categoryId, Name = "Test Category" };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mockCategoryRepository
            .Setup(r => r.DeleteAsync(categoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _categoryService.DeleteAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Message.Should().Contain("eliminada exitosamente");
    }

    [Fact]
    public async Task DeleteAsync_NonExistingCategory_ReturnsNotFoundResponse()
    {
        // Arrange
        var categoryId = 999;

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.DeleteAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
    }
}

