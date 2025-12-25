using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ServerCloudStore.Application.DTOs.BulkImport;
using ServerCloudStore.Application.Services;
using ServerCloudStore.Domain.Entities;
using ServerCloudStore.Domain.Repositories;
using ServerCloudStore.Domain.Services;

namespace ServerCloudStore.Tests.Unit.Services;

public class BulkImportServiceTests
{
    private readonly Mock<IBulkImportJobRepository> _mockJobRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<ICsvProcessor> _mockCsvProcessor;
    private readonly Mock<IDataGenerator> _mockDataGenerator;
    private readonly Mock<ILogger<BulkImportService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BulkImportService _bulkImportService;

    public BulkImportServiceTests()
    {
        _mockJobRepository = new Mock<IBulkImportJobRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockCsvProcessor = new Mock<ICsvProcessor>();
        _mockDataGenerator = new Mock<IDataGenerator>();
        _mockLogger = new Mock<ILogger<BulkImportService>>();
        _mockMapper = new Mock<IMapper>();

        _bulkImportService = new BulkImportService(
            _mockJobRepository.Object,
            _mockProductRepository.Object,
            _mockCategoryRepository.Object,
            _mockNotificationService.Object,
            _mockCsvProcessor.Object,
            _mockDataGenerator.Object,
            _mockLogger.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task StartImportAsync_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new BulkImportRequestDto { GenerateCount = 100 };
        var jobDto = new BulkImportJobDto { Id = Guid.NewGuid(), Status = "Pending" };
        var jobId = Guid.NewGuid();

        _mockJobRepository
            .Setup(r => r.CreateAsync(It.IsAny<BulkImportJob>()))
            .ReturnsAsync(jobId);

        _mockMapper
            .Setup(m => m.Map<BulkImportJobDto>(It.IsAny<BulkImportJob>()))
            .Returns(jobDto);

        // Act
        var result = await _bulkImportService.StartImportAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(202);
        result.Message.Should().Contain("Importación iniciada");
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetJobStatusAsync_ExistingJob_ReturnsSuccessResponse()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new BulkImportJob
        {
            Id = jobId,
            Status = "Processing",
            TotalRecords = 100,
            ProcessedRecords = 50
        };
        var jobDto = new BulkImportJobDto
        {
            Id = jobId,
            Status = "Processing"
        };

        _mockJobRepository
            .Setup(r => r.GetByIdAsync(jobId))
            .ReturnsAsync(job);

        _mockMapper
            .Setup(m => m.Map<BulkImportJobDto>(job))
            .Returns(jobDto);

        // Act
        var result = await _bulkImportService.GetJobStatusAsync(jobId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Code.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be("Processing");
    }

    [Fact]
    public async Task GetJobStatusAsync_NonExistingJob_ReturnsNotFoundResponse()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _mockJobRepository
            .Setup(r => r.GetByIdAsync(jobId))
            .ReturnsAsync((BulkImportJob?)null);

        // Act
        var result = await _bulkImportService.GetJobStatusAsync(jobId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(404);
        result.Message.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task GetAllJobsAsync_ReturnsAllJobs()
    {
        // Arrange
        var jobs = new List<BulkImportJob>
        {
            new BulkImportJob { Id = Guid.NewGuid(), Status = "Completed" },
            new BulkImportJob { Id = Guid.NewGuid(), Status = "Processing" }
        };
        var jobDtos = new List<BulkImportJobDto>
        {
            new BulkImportJobDto { Id = jobs[0].Id, Status = "Completed" },
            new BulkImportJobDto { Id = jobs[1].Id, Status = "Processing" }
        };

        _mockJobRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(jobs);

        _mockMapper
            .Setup(m => m.Map<List<BulkImportJobDto>>(jobs))
            .Returns(jobDtos);

        // Act
        var result = await _bulkImportService.GetAllJobsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task StartImportAsync_ExceptionThrown_ReturnsErrorResponse()
    {
        // Arrange
        var request = new BulkImportRequestDto { GenerateCount = 100 };

        _mockJobRepository
            .Setup(r => r.CreateAsync(It.IsAny<BulkImportJob>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _bulkImportService.StartImportAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be(500);
        result.Message.Should().Contain("Error al iniciar la importación");
    }
}

