using APIs.DTOs;
using APIs.Models;

namespace APIs.Repositories;

public interface IJobListingRepository
{
    Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync();

    Task<JobDetailResponse?> GetJobListingDetailAsync(Guid id);

    Task<JobListing?> GetJobListingByIdAsync(Guid id);

    Task<bool> JobListingExistsAsync(Guid id);

    Task<bool> IsJobListingOpenAsync(Guid id);

    Task<bool> DuplicateJobExistsAsync(string title, string companyName);

    Task CreateJobListingAsync(JobListing listing, string companyName, string industry);

    Task UpdateJobListingAsync(JobListing listing, string companyName, string industry);

    Task CloseJobListingAsync(Guid id);

    Task DeleteJobListingAsync(Guid id);

    // Text search
    Task<IEnumerable<JobListResponse>> SearchAsync(string searchTerm);

    // Application statistics with RANK() window function
    Task<IEnumerable<JobListingStatsResponse>> GetApplicationStatsAsync(Guid companyId);
}
