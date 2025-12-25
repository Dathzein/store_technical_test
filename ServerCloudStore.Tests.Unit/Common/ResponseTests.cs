using FluentAssertions;
using ServerCloudStore.Transversal.Common;

namespace ServerCloudStore.Tests.Unit.Common;

public class ResponseTests
{
    [Fact]
    public void Success_WithData_ShouldCreateSuccessResponse()
    {
        // Arrange
        var data = "Test Data";

        // Act
        var response = Response<string>.Success(data, "Success message", 200);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().Be(data);
        response.Message.Should().Be("Success message");
        response.Code.Should().Be(200);
    }

    [Fact]
    public void Error_WithMessage_ShouldCreateErrorResponse()
    {
        // Act
        var response = Response<string>.Error("Error message", 400);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Message.Should().Be("Error message");
        response.Code.Should().Be(400);
    }

    [Fact]
    public void Success_WithoutMessage_ShouldUseDefaultMessage()
    {
        // Arrange
        var data = 42;

        // Act
        var response = Response<int>.Success(data);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().Be(42);
        response.Message.Should().Be("Operación exitosa");
        response.Code.Should().Be(200);
    }

    [Fact]
    public void Error_WithoutCode_ShouldUseDefaultCode()
    {
        // Act
        var response = Response<string>.Error("Error occurred");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeFalse();
        response.Message.Should().Be("Error occurred");
        response.Code.Should().Be(400);
    }
}

