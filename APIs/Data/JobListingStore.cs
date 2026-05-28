//This class contains separate abstracted dummy data for job listings 
// to make my routing and endpoint code clean

using APIs.Models;

namespace APIs.Data;

public class JobListingStore
{
    // Static in-memory list acting as temporary dummy database data
    private static readonly List<JobListing> Jobs =
    [
        new JobListing
        {
            Id = Guid.NewGuid(),
            Title = "Software Developer intern",
            Description = "24 month Software Developer internship that provides candidates with handson experiences",
            Company = "Tech2day Software",
            Location = "CapeTown",
            Type = "Full-Time",
            PostedAt = DateTime.UtcNow,
            IsActive = true
        },

        new JobListing
        {
            Id = Guid.NewGuid(),
            Title = "Senior Lead Developer",
            Description = "Lead the software development team and assign code tasks as a responsibility",
            Company = "Ali Code",
            Location = "Johannesburg",
            Type = "Part-Time",
            PostedAt = DateTime.UtcNow,
            IsActive = true
        },

        new JobListing
        {
            Id = Guid.NewGuid(),
            Title = "UI/UX Designer",
            Description = "Design applications",
            Company = "Creative Labs",
            Location = "Cape Town",
            Type = "Contract",
            PostedAt = DateTime.UtcNow,
            IsActive = true
        }
    ];

    // Returns all available job listings
    public async Task<List<JobListing>> GetAllJobsAsync()
    {
        await Task.CompletedTask;
        return Jobs;
    }

    // Returns a single job listing by ID
    public async Task<JobListing?> GetJobByIdAsync(Guid id)
    {
        await Task.CompletedTask;

        return Jobs.FirstOrDefault(job => job.Id == id);
    }
}