using APIs.DTOs;
using APIs.Exceptions;
using APIs.Models;
using APIs.Repositories;
using APIs.Services;
using NSubstitute;
using Xunit;

namespace API.Tests.Unit.Services;

public class JobListingServiceTests
{
    private readonly IJobListingRepository _repository;
    private readonly JobListingService _sut;

    public JobListingServiceTests()
    {
        _repository = Substitute.For<IJobListingRepository>();
        _sut = new JobListingService(_repository);
    }

    [Fact]
    public async Task CreateAsync_WhenSalaryMaxLessThanSalaryMin_ThrowsInvalidSalaryException()
    {
        // Arrange
        var request = new CreateJobRequest(
            Title: "Software Developer",
            CompanyName: "Acme Corp",
            Industry: "Tech",
            Location: "Johannesburg",
            Description: "An expansive software position with competitive metrics.",
            Type: JobType.FullTime,
            ClosingDate: DateTime.UtcNow.AddDays(30),
            SalaryMin: 80000,
            SalaryMax: 50000
        );

        // Act
        var act = () => _sut.CreateJobListingAsync(request);

        // Assert
        await Assert.ThrowsAsync<InvalidSalaryRangeException>(act);
        await _repository.DidNotReceive().CreateJobListingAsync(Arg.Any<JobListing>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task CreateAsync_WhenExpiresAtIsInThePast_ThrowsInvalidListingException()
    {
        // Arrange
        var request = new CreateJobRequest(
            Title: "Software Developer",
            CompanyName: "Acme Corp",
            Industry: "Tech",
            Location: "Johannesburg",
            Description: "An expansive software position with competitive metrics.",
            Type: JobType.FullTime,
            ClosingDate: DateTime.UtcNow.AddDays(-1),
            SalaryMin: 40000,
            SalaryMax: 60000
        );

        // Act
        var act = () => _sut.CreateJobListingAsync(request);

        // Assert
        await Assert.ThrowsAsync<InvalidExpiryDateException>(act);
        await _repository.DidNotReceive().CreateJobListingAsync(Arg.Any<JobListing>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task CreateAsync_WhenValid_CallsAddAsyncExactlyOnce()
    {
        // Arrange
        var request = new CreateJobRequest(
            Title: "Software Developer",
            CompanyName: "Acme Corp",
            Industry: "Tech",
            Location: "Johannesburg",
            Description: "An expansive software position with competitive metrics.",
            Type: JobType.FullTime,
            ClosingDate: DateTime.UtcNow.AddDays(15),
            SalaryMin: 50000,
            SalaryMax: 70000
        );
        _repository.DuplicateJobExistsAsync(request.Title, request.CompanyName).Returns(false);

        // Act
        await _sut.CreateJobListingAsync(request);

        // Assert
        await _repository.Received(1).CreateJobListingAsync(Arg.Any<JobListing>(), request.CompanyName, Arg.Any<string>());
    }

    [Fact]
    public async Task PatchAsync_WhenOnlySalaryMinChanged_CallsValidation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = new JobListing
        {
            Id = id,
            Title = "DevOps Engineer",
            SalaryMin = 40000,
            SalaryMax = 60000,
            IsActive = true,
            ClosingDate = DateTime.UtcNow.AddDays(10)
        };
        _repository.GetJobListingByIdAsync(id).Returns(existing);

        var request = new UpdateJobListingRequest { SalaryMin = 75000 };

        // Act
        var act = () => _sut.PatchAsync(id, request);

        // Assert
        await Assert.ThrowsAsync<InvalidSalaryRangeException>(act);
        await _repository.DidNotReceive().PatchAsync(id, Arg.Any<UpdateJobListingRequest>());
    }

    [Fact]
    public async Task PatchAsync_WhenOnlyTitleChanged_DoesNotCallSalaryValidation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = new JobListing
        {
            Id = id,
            Title = "System Engineer",
            SalaryMin = 40000,
            SalaryMax = 60000
        };
        _repository.GetJobListingByIdAsync(id).Returns(existing);

        var request = new UpdateJobListingRequest { Title = "Lead Architect" };
        _repository.PatchAsync(id, request).Returns(new JobListResponse(id, "Lead Architect", "Acme Corp", "JHB", DateTime.UtcNow, "R40k-R60k", DateTime.UtcNow.AddDays(5), 0));

        // Act
        var result = await _sut.PatchAsync(id, request);

        // Assert
        Assert.NotNull(result);
        await _repository.Received(1).PatchAsync(id, request);
    }

    [Fact]
    public async Task PatchAsync_WhenListingNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetJobListingByIdAsync(id).Returns((JobListing?)null);

        var request = new UpdateJobListingRequest { Title = "Cloud Guru" };

        // Act
        var act = () => _sut.PatchAsync(id, request);

        // Assert
        await Assert.ThrowsAsync<JobNotFoundException>(act);
        await _repository.DidNotReceive().PatchAsync(id, Arg.Any<UpdateJobListingRequest>());
    }
}