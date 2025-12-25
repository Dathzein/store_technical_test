using FluentAssertions;
using FluentValidation.TestHelper;
using ServerCloudStore.Application.DTOs.Product;
using ServerCloudStore.Application.Validators;

namespace ServerCloudStore.Tests.Unit.Validators;

public class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator;

    public CreateProductDtoValidatorTests()
    {
        _validator = new CreateProductDtoValidator();
    }

    [Fact]
    public void Validate_ValidProduct_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NegativePrice_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Price = -10,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_NegativeStock_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Price = 100,
            Stock = -5,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock);
    }

    [Fact]
    public void Validate_InvalidCategoryId_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Price = 100,
            Stock = 10,
            CategoryId = 0
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }
}

