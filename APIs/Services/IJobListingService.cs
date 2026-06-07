// using APIs.DTOs;
// using APIs.Models;

// namespace APIs.Services;

// public interface IJobListingService
// {
//     Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync();
//     Task<JobDetailResponse> GetJobListingDetailAsync(Guid id);
//     Task<bool> JobListingExistsAsync(Guid id);
//     Task<bool> IsJobListingOpenAsync(Guid id);
//     Task<bool> DuplicateJobExistsAsync(string title, string companyName, Guid companyId);
//     Task<Company?> GetCompanyByNameAsync(string companyName);
//     Task<JobListResponse> CreateJobListingAsync(CreateJobRequest request);
//     Task<JobListResponse> UpdateJobListingAsync(Guid id, UpdateJobRequest request);
//     Task CloseJobListingAsync(Guid id);
//     Task DeleteJobListingAsync(Guid id);
// }

// // using APIs.DTOs;
// // using APIs.Models;

// // namespace APIs.Services;

// // public interface IJobListingService
// // {
// //     Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync();

// //     Task<JobDetailResponse?> GetJobListingDetailAsync(Guid id);
// //     Task<JobDetailResponse> GetByJobListingIdAsync(Guid id);

// //     Task<bool> JobListingExistsAsync(Guid id);

// //     Task<bool> IsJobListingOpenAsync(Guid id);

// //     Task<bool> DuplicateJobExistsAsync(string title, string companyName, Guid companyId);

// //     Task<Company?> GetCompanyByNameAsync(string companyName);
// //     // Task<JobListResponse> CreateJobListingAsync(CreateJobRequest request);
// //     Task CreateJobListingAsync(JobListing listing, Company? newCompany = null);
// //     // Task<JobListResponse> UpdateJobListingAsync(Guid id, UpdateJobRequest request);
// //     Task UpdateJobListingAsync(JobListing listing, Company? newCompany = null);

// //     Task DeleteJobListingAsync(Guid id);
// //     Task CloseJobListingAsync(Guid id);
// // }


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
}
