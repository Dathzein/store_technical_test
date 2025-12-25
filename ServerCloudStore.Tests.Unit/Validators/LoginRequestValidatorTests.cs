using FluentAssertions;
using FluentValidation.TestHelper;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Application.Validators;

namespace ServerCloudStore.Tests.Unit.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator;

    public LoginRequestValidatorTests()
    {
        _validator = new LoginRequestValidator();
    }

    [Fact]
    public void Validate_ValidLoginRequest_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUsername_ShouldHaveError()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "",
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Validate_EmptyPassword_ShouldHaveError()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "admin",
            Password = ""
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}

