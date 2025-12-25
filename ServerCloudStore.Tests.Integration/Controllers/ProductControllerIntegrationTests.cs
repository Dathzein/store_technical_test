using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Application.DTOs.Category;
using ServerCloudStore.Application.DTOs.Product;
using ServerCloudStore.Transversal.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServerCloudStore.Tests.Integration.Controllers;

public class ProductControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private string? _authToken;

    public ProductControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
    public async Task GetProducts_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Product");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProducts_WithAuth_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await _client.GetAsync("/api/Product?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<object>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Primero crear una categoría
        var categoryDto = new CreateCategoryDto
        {
            Name = "Test Category for Product",
            Description = "Test"
        };
        var categoryResponse = await _client.PostAsJsonAsync("/api/Category", categoryDto);
        var categoryResult = await categoryResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        var categoryId = categoryResult?.Data?.Id ?? 1;

        var productDto = new CreateProductDto
        {
            Name = "Integration Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Stock = 50,
            CategoryId = categoryId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Product", productDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<Response<ProductDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(productDto.Name);
    }

    [Fact]
    public async Task GetProductById_ExistingProduct_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Crear un producto primero
        var categoryDto = new CreateCategoryDto { Name = "Test Cat", Description = "Test" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/Category", categoryDto);
        var categoryResult = await categoryResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        
        var productDto = new CreateProductDto
        {
            Name = "Test Product for Get",
            Description = "Test",
            Price = 50,
            Stock = 10,
            CategoryId = categoryResult?.Data?.Id ?? 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Product", productDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Response<ProductDto>>();
        var productId = createResult?.Data?.Id ?? 0;

        // Act
        var response = await _client.GetAsync($"/api/Product/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<ProductDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(productId);
    }

    [Fact]
    public async Task GetProductById_NonExisting_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await _client.GetAsync("/api/Product/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_ValidData_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Crear producto
        var categoryDto = new CreateCategoryDto { Name = "Update Test Cat", Description = "Test" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/Category", categoryDto);
        var categoryResult = await categoryResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        
        var productDto = new CreateProductDto
        {
            Name = "Original Name",
            Description = "Original",
            Price = 100,
            Stock = 10,
            CategoryId = categoryResult?.Data?.Id ?? 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Product", productDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Response<ProductDto>>();
        var productId = createResult?.Data?.Id ?? 0;

        var updateDto = new UpdateProductDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 150,
            Stock = 20,
            CategoryId = categoryResult?.Data?.Id ?? 1
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Product/{productId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<ProductDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteProduct_ExistingProduct_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Crear producto
        var categoryDto = new CreateCategoryDto { Name = "Delete Test Cat", Description = "Test" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/Category", categoryDto);
        var categoryResult = await categoryResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();
        
        var productDto = new CreateProductDto
        {
            Name = "To Delete",
            Description = "Will be deleted",
            Price = 50,
            Stock = 5,
            CategoryId = categoryResult?.Data?.Id ?? 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Product", productDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Response<ProductDto>>();
        var productId = createResult?.Data?.Id ?? 0;

        // Act
        var response = await _client.DeleteAsync($"/api/Product/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verificar que fue eliminado
        var getResponse = await _client.GetAsync($"/api/Product/{productId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

