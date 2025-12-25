using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Transversal.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServerCloudStore.Tests.Integration.Controllers;

public class CategoryControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private string? _authToken;

    public CategoryControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        if (_authToken != null)
            return _authToken;

        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        };

        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to authenticate");

        var result = await response.Content.ReadFromJsonAsync<Response<LoginResponseDto>>();
        _authToken = result?.Data?.Token ?? throw new Exception("Token not found");
        
        return _authToken;
    }

    private void SetAuthorizationHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetCategories_WithAuth_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await _client.GetAsync("/api/Category");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<List<CategoryDto>>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        var categoryDto = new CreateCategoryDto
        {
            Name = $"Integration Test Category {Guid.NewGuid()}",
            Description = "Test Description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Category", categoryDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(categoryDto.Name);
    }

    [Fact]
    public async Task GetCategoryById_ExistingCategory_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Crear categoría primero
        var categoryDto = new CreateCategoryDto
        {
            Name = $"Test Category {Guid.NewGuid()}",
            Description = "Test"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Category", categoryDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        var categoryId = createResult?.Data?.Id ?? 0;

        // Act
        var response = await _client.GetAsync($"/api/Category/{categoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(categoryId);
    }

    [Fact]
    public async Task UpdateCategory_ValidData_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Crear categoría
        var categoryDto = new CreateCategoryDto
        {
            Name = $"Original Name {Guid.NewGuid()}",
            Description = "Original"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Category", categoryDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        var categoryId = createResult?.Data?.Id ?? 0;

        var updateDto = new UpdateCategoryDto
        {
            Name = $"Updated Name {Guid.NewGuid()}",
            Description = "Updated Description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Category/{categoryId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be(updateDto.Name);
    }

    [Fact]
    public async Task DeleteCategory_ExistingCategory_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Crear categoría
        var categoryDto = new CreateCategoryDto
        {
            Name = $"To Delete {Guid.NewGuid()}",
            Description = "Will be deleted"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Category", categoryDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        var categoryId = createResult?.Data?.Id ?? 0;

        // Act
        var response = await _client.DeleteAsync($"/api/Category/{categoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verificar que fue eliminada
        var getResponse = await _client.GetAsync($"/api/Category/{categoryId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCategoryById_NonExisting_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await _client.GetAsync("/api/Category/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

