using APIs.DTOs;
using APIs.Exceptions;
using APIs.Models;
using APIs.Repositories;
using APIs.Services;
using NSubstitute;
using Xunit;

namespace API.Tests.Unit.Services;

public class ApplicationServiceTests
{
    private readonly IApplicationRepository _appRepo;
    private readonly IJobListingRepository _jobRepo;
    private readonly ApplicationService _sut;

    public ApplicationServiceTests()
    {
        _appRepo = Substitute.For<IApplicationRepository>();
        _jobRepo = Substitute.For<IJobListingRepository>();
        _sut = new ApplicationService(_appRepo, _jobRepo);
    }

    [Theory]
    [InlineData("Submitted", "UnderReview")]
    [InlineData("UnderReview", "Shortlisted")]
    [InlineData("UnderReview", "Rejected")]
    [InlineData("Shortlisted", "Offered")]
    [InlineData("Shortlisted", "Rejected")]
    public async Task UpdateApplicationStatusAsync_WhenTransitionIsLegal_CallsUpdateApplicationStatusAsync(string from, string to)
    {
        // Arrange
        var jobListingId = Guid.NewGuid();
        var applicantId = Guid.NewGuid();
        var existingApplication = new ApplicationResponse(jobListingId, jobListingId, applicantId, "Title", "Name", DateTime.UtcNow, from);

        _appRepo.GetApplicationsListAsync(jobListingId).Returns(new List<ApplicationResponse> { existingApplication });
        var request = new UpdateApplicationRequest(to);

        // Act
        await _sut.UpdateApplicationStatusAsync(jobListingId, applicantId, request);

        // Assert
        var parsedStatus = Enum.Parse<ApplicationStatus>(to);
        await _appRepo.Received(1).UpdateApplicationStatusAsync(jobListingId, applicantId, parsedStatus);
    }

    [Theory]
    [InlineData("Rejected", "Submitted")]
    [InlineData("Offered", "Submitted")]
    [InlineData("Rejected", "UnderReview")]
    [InlineData("Offered", "Shortlisted")]
    public async Task UpdateApplicationStatusAsync_WhenTransitionIsIllegal_ThrowsInvalidStatusTransitionException(string from, string to)
    {
        // Arrange
        var jobListingId = Guid.NewGuid();
        var applicantId = Guid.NewGuid();
        var existingApplication = new ApplicationResponse(jobListingId, jobListingId, applicantId, "Title", "Name", DateTime.UtcNow, from);

        _appRepo.GetApplicationsListAsync(jobListingId).Returns(new List<ApplicationResponse> { existingApplication });
        var request = new UpdateApplicationRequest(to); 

        // Act
        var act = () => _sut.UpdateApplicationStatusAsync(jobListingId, applicantId, request);

        // Assert
        await Assert.ThrowsAsync<InvalidStatusTransitionException>(act);
        var parsedStatus = Enum.Parse<ApplicationStatus>(to); 
        await _appRepo.DidNotReceive().UpdateApplicationStatusAsync(jobListingId, applicantId, parsedStatus);
    }

    [Fact]
    public async Task UpdateApplicationStatusAsync_WhenApplicationNotFound_ThrowsApplicationNotFoundException()
    {
        // Arrange
        var jobListingId = Guid.NewGuid();
        var applicantId = Guid.NewGuid();
        _appRepo.GetApplicationsListAsync(jobListingId).Returns(new List<ApplicationResponse>());

        var request = new UpdateApplicationRequest("UnderReview");

        // Act
        var act = () => _sut.UpdateApplicationStatusAsync(jobListingId, applicantId, request);

        // Assert
        await Assert.ThrowsAsync<ApplicationNotFoundException>(act);
    }
}