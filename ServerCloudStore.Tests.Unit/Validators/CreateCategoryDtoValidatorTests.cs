using FluentAssertions;
using FluentValidation.TestHelper;
using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Application.Validators;

namespace ServerCloudStore.Tests.Unit.Validators;

public class CreateCategoryDtoValidatorTests
{
    private readonly CreateCategoryDtoValidator _validator;

    public CreateCategoryDtoValidatorTests()
    {
        _validator = new CreateCategoryDtoValidator();
    }

    [Fact]
    public void Validate_ValidCategory_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test Description"
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
        var dto = new CreateCategoryDto
        {
            Name = "",
            Description = "Test Description"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}

