using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Services;

namespace ServerCloudStore.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IAuthDomainService> _mockAuthDomainService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockAuthDomainService = new Mock<IAuthDomainService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockMapper = new Mock<IMapper>();

        // Configurar valores de configuración por defecto
        _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"])
            .Returns("SuperSecretKeyForJWTTokenGenerationMinimum32Characters!");
        _mockConfiguration.Setup(c => c["JwtSettings:Issuer"])
            .Returns("ServerCloudStoreAPI");
        _mockConfiguration.Setup(c => c["JwtSettings:Audience"])
            .Returns("ServerCloudStoreClient");
        _mockConfiguration.Setup(c => c.GetSection("JwtSettings:ExpirationMinutes").Value)
            .Returns("60");

        _authService = new AuthService(
            _mockAuthDomainService.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessResponse()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Username = "admin", Password = "admin123" };
        var user = new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@test.com",
            RoleId = 1,
            Role = new Role
            {
                Id = 1,
                Name = "Admin",
                Permissions = "[\"ReadProducts\",\"WriteProducts\"]"
            }
        };
        var userDto = new UserDto
        {
            Id = 1,
            Username = "admin",
            Email = "admin@test.com"
        };

        _mockAuthDomainService
            .Setup(s => s.ValidateCredentialsAsync(loginRequest.Username, loginRequest.Password))
            .ReturnsAsync(user);

        _mockMapper
            .Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.User.Should().NotBeNull();
        result.Data.User.Username.Should().Be("admin");
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsErrorResponse()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Username = "invalid", Password = "wrong" };

        _mockAuthDomainService
            .Setup(s => s.ValidateCredentialsAsync(loginRequest.Username, loginRequest.Password))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(401);
        result.Message.Should().Be("Credenciales inválidas");
    }

    [Fact]
    public async Task LoginAsync_ExceptionThrown_ReturnsErrorResponse()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Username = "admin", Password = "admin123" };

        _mockAuthDomainService
            .Setup(s => s.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(500);
        result.Message.Should().Contain("Error al procesar la autenticación");
    }

    [Fact]
    public async Task LoginAsync_ValidUser_TokenContainsClaims()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Username = "admin", Password = "admin123" };
        var user = new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@test.com",
            RoleId = 1,
            Role = new Role
            {
                Id = 1,
                Name = "Admin",
                Permissions = "[\"ReadProducts\",\"WriteProducts\"]"
            }
        };
        var userDto = new UserDto
        {
            Id = 1,
            Username = "admin",
            Email = "admin@test.com"
        };

        _mockAuthDomainService
            .Setup(s => s.ValidateCredentialsAsync(loginRequest.Username, loginRequest.Password))
            .ReturnsAsync(user);

        _mockMapper
            .Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}

