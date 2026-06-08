using APIs.DTOs;

namespace APIs.Services;

public interface IJobListingService
{
    Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync();
    Task<JobDetailResponse> GetJobListingDetailAsync(Guid id);
    Task<bool> JobListingExistsAsync(Guid id);
    Task<bool> IsJobListingOpenAsync(Guid id);
    Task<bool> DuplicateJobExistsAsync(string title, string companyName);
    Task<JobListResponse> CreateJobListingAsync(CreateJobRequest request);
    Task<JobListResponse> UpdateJobListingAsync(Guid id, UpdateJobRequest request);
    Task CloseJobListingAsync(Guid id);
    Task DeleteJobListingAsync(Guid id);

    // Text search
    Task<IEnumerable<JobListResponse>> SearchAsync(string searchTerm);

    // Application statistics
    Task<IEnumerable<JobListingStatsResponse>> GetApplicationStatsAsync(Guid companyId);
}
