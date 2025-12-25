using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ServerCloudStore.Application.DTOs.Auth;
using ServerCloudStore.Application.DTOs.BulkImport;
using ServerCloudStore.Transversal.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServerCloudStore.Tests.Integration.Controllers;

public class BulkImportControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private string? _authToken;

    public BulkImportControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
    public async Task StartImport_WithGenerateCount_ReturnsAccepted()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        var content = new MultipartFormDataContent();
        content.Add(new StringContent("10"), "GenerateCount");

        // Act
        var response = await _client.PostAsync("/api/BulkImport", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var result = await response.Content.ReadFromJsonAsync<Response<BulkImportJobDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().NotBeEmpty();
        result.Data.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task GetJobStatus_ExistingJob_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Crear un job primero
        var content = new MultipartFormDataContent();
        content.Add(new StringContent("5"), "GenerateCount");
        var createResponse = await _client.PostAsync("/api/BulkImport", content);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Response<BulkImportJobDto>>();
        var jobId = createResult?.Data?.Id ?? Guid.Empty;

        // Esperar un momento para que el job se procese
        await Task.Delay(500);

        // Act
        var response = await _client.GetAsync($"/api/BulkImport/{jobId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<BulkImportJobDto>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(jobId);
    }

    [Fact]
    public async Task GetAllJobs_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await _client.GetAsync("/api/BulkImport");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<List<BulkImportJobDto>>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetJobStatus_NonExistingJob_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthorizationHeader(token);
        var nonExistingJobId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/BulkImport/{nonExistingJobId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task StartImport_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        content.Add(new StringContent("10"), "GenerateCount");

        // Act
        var response = await _client.PostAsync("/api/BulkImport", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

