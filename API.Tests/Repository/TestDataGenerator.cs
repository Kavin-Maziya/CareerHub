using APIs.Models;
using System;

namespace API.Tests.Helpers;

public static class TestDataGenerator
{
    private static readonly Random _random = new Random();
    private static readonly string[] _titles = { "Software Engineer", "DevOps Specialist", "Product Manager", "Data Scientist", "Frontend Developer", "Architect" };
    private static readonly string[] _locations = { "Johannesburg", "Cape Town", "Durban", "Pretoria", "Stellenbosch" };
    private static readonly string[] _companies = { "Acme Corp", "Globex", "Soylent Corp", "Initech", "Umbrella Corp", "Cyberdyne" };
    private static readonly string[] _industries = { "Technology", "Finance", "Healthcare", "Manufacturing" };

    public static Company GenerateCompany()
    {
        return new Company
        {
            CompanyId = Guid.NewGuid(),
            CompanyName = $"{_companies[_random.Next(_companies.Length)]} {Guid.NewGuid().ToString()[..4]}",
            Industry = _industries[_random.Next(_industries.Length)]
        };
    }

    public static JobListing GenerateJobListing(Guid companyId, bool isActive = true, string? title = null, DateTime? postedAt = null, DateTime? closingDate = null)
    {
        var now = DateTime.UtcNow;
        return new JobListing
        {
            Id = Guid.NewGuid(),
            Title = title ?? _titles[_random.Next(_titles.Length)],
            CompanyId = companyId,
            IsActive = isActive,
            PostedAt = postedAt ?? now.AddDays(-_random.Next(0, 10)),
            ClosingDate = closingDate ?? now.AddDays(_random.Next(5, 30)),
            Location = _locations[_random.Next(_locations.Length)],
            Description = "This is a randomized job description for a highly sought-after position in our growing team."
        };
    }

    public static Applicant GenerateApplicant(string? email = null)
    {
        var id = Guid.NewGuid().ToString()[..4];
        return new Applicant
        {
            Id = Guid.NewGuid(),
            FirstName = $"User_{id}",
            LastName = "Candidate",
            Email = email ?? $"candidate_{id}@careerhub.com"
        };
    }

    public static Application GenerateApplication(Guid jobListingId, Guid applicantId)
    {
        return new Application
        {
            JobListingId = jobListingId,
            ApplicantId = applicantId,
            Status = ApplicationStatus.Submitted,
            SubmittedAt = DateTime.UtcNow
        };
    }
}