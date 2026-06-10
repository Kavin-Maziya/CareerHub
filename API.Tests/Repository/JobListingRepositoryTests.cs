using APIs.Data;
using APIs.DTOs;
using APIs.Models;
using APIs.Repositories;
using Microsoft.EntityFrameworkCore;
using API.Tests.Helpers;
using Xunit;

namespace API.Tests.Repository;

public class JobListingRepositoryTests : IClassFixture<PostgreSqlContainerFixture>
{
    private readonly PostgreSqlContainerFixture _fixture;

    public JobListingRepositoryTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    private CareerHubDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CareerHubDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;

        var context = new CareerHubDbContext(options);
        context.Database.Migrate();
        
        context.Applications.RemoveRange(context.Applications);
        context.JobListings.RemoveRange(context.JobListings);
        context.Companies.RemoveRange(context.Companies);
        context.Applicants.RemoveRange(context.Applicants);
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetActiveListingsPagedAsync_Page1_ReturnsCorrectCount()
    {
        using var context = CreateContext();
        var repo = new JobListingRepository(context);

        var company = TestDataGenerator.GenerateCompany();
        context.Companies.Add(company);

        for (int i = 0; i < 6; i++)
        {
            context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId));
        }
        await context.SaveChangesAsync();

        var result = await repo.GetActiveListingsPagedAsync(1, 4, new JobListingFilterQuery());

        Assert.Equal(4, result.Data.Count());
        Assert.Equal(6, result.TotalCount);
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task GetActiveListingsPagedAsync_Page2_ReturnsDifferentRows()
    {
        using var context = CreateContext();
        var repo = new JobListingRepository(context);

        var company = TestDataGenerator.GenerateCompany();
        context.Companies.Add(company);

        for (int i = 0; i < 6; i++)
        {
            context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId));
        }
        await context.SaveChangesAsync();

        var p1 = await repo.GetActiveListingsPagedAsync(1, 3, new JobListingFilterQuery());
        var p2 = await repo.GetActiveListingsPagedAsync(2, 3, new JobListingFilterQuery());

        var p1Ids = p1.Data.Select(x => x.Id).ToHashSet();
        var p2Ids = p2.Data.Select(x => x.Id).ToHashSet();

        p1Ids.IntersectWith(p2Ids);
        Assert.Empty(p1Ids);
    }

    [Fact]
    public async Task GetActiveListingsPagedAsync_ResultsAreOrderedByPostedAtDescending()
    {
        using var context = CreateContext();
        var repo = new JobListingRepository(context);

        var company = TestDataGenerator.GenerateCompany();
        context.Companies.Add(company);

        var now = DateTime.UtcNow;
        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, title: "Old", postedAt: now.AddDays(-2)));
        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, title: "New", postedAt: now));
        await context.SaveChangesAsync();

        var result = await repo.GetActiveListingsPagedAsync(1, 10, new JobListingFilterQuery());
        var listings = result.Data.ToList();

        Assert.Equal("New", listings[0].Title);
        Assert.Equal("Old", listings[1].Title);
    }

    [Fact]
    public async Task GetActiveListingsPagedAsync_ExcludesExpiredListings()
    {
        using var context = CreateContext();
        var repo = new JobListingRepository(context);

        var company = TestDataGenerator.GenerateCompany();
        context.Companies.Add(company);

        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, closingDate: DateTime.UtcNow.AddDays(5)));
        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, closingDate: DateTime.UtcNow.AddDays(5)));
        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, closingDate: DateTime.UtcNow.AddDays(5)));
        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, closingDate: DateTime.UtcNow.AddDays(-5)));
        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, closingDate: DateTime.UtcNow.AddDays(-1)));
        await context.SaveChangesAsync();

        var result = await repo.GetActiveListingsPagedAsync(1, 20, new JobListingFilterQuery());
        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task CheckConstraint_RejectsSalaryMaxLessThanSalaryMin()
    {
        using var context = CreateContext();
        var company = TestDataGenerator.GenerateCompany();
        context.Companies.Add(company);

        var invalidListing = TestDataGenerator.GenerateJobListing(company.CompanyId);
        invalidListing.SalaryMin = 50000;
        invalidListing.SalaryMax = 20000;
        context.JobListings.Add(invalidListing);

        await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task CheckConstraint_RejectsExpiresAtBeforeCreatedAt()
    {
        using var context = CreateContext();
        var company = TestDataGenerator.GenerateCompany();
        context.Companies.Add(company);

        var invalidListing = TestDataGenerator.GenerateJobListing(company.CompanyId);
        invalidListing.PostedAt = DateTime.UtcNow;
        invalidListing.ClosingDate = DateTime.UtcNow.AddDays(-5);
        context.JobListings.Add(invalidListing);

        await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task HasAppliedAsync_WhenApplicationExists_ReturnsTrue()
    {
        using var context = CreateContext();
        var repo = new ApplicationRepository(context);

        var company = TestDataGenerator.GenerateCompany();
        var listing = TestDataGenerator.GenerateJobListing(company.CompanyId);
        var applicant = TestDataGenerator.GenerateApplicant();
        var app = TestDataGenerator.GenerateApplication(listing.Id, applicant.Id);

        context.Companies.Add(company);
        context.JobListings.Add(listing);
        context.Applicants.Add(applicant);
        context.Applications.Add(app);
        await context.SaveChangesAsync();

        var hasApplied = await repo.HasApplicantAlreadyAppliedAsync(listing.Id, applicant.Id);
        Assert.True(hasApplied);
    }

    [Fact]
    public async Task HasAppliedAsync_WhenNoApplicationExists_ReturnsFalse()
    {
        using var context = CreateContext();
        var repo = new ApplicationRepository(context);

        var hasApplied = await repo.HasApplicantAlreadyAppliedAsync(Guid.NewGuid(), Guid.NewGuid());
        Assert.False(hasApplied);
    }

    [Fact]
    public async Task FullTextSearchAsync_ReturnsStemmedMatches()
    {
        using var context = CreateContext();
        var repo = new JobListingRepository(context);

        var company = TestDataGenerator.GenerateCompany();
        context.Companies.Add(company);

        var listing = TestDataGenerator.GenerateJobListing(company.CompanyId, title: "Software Engineering Position");
        context.JobListings.Add(listing);
        await context.SaveChangesAsync();

        var matches = await repo.SearchAsync("engineer");
        Assert.Single(matches);
    }

    [Fact]
    public async Task FullTextSearchAsync_DoesNotReturnNonMatchingListings()
    {
        using var context = CreateContext();
        var repo = new JobListingRepository(context);

        var company = TestDataGenerator.GenerateCompany();
        context.Companies.Add(company);

        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, title: "Accountant Role"));
        context.JobListings.Add(TestDataGenerator.GenerateJobListing(company.CompanyId, title: "Bookkeeper Clerk"));
        await context.SaveChangesAsync();

        var matches = await repo.SearchAsync("engineer");
        Assert.Empty(matches);
    }
}