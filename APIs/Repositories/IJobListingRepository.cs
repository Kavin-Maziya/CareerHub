// // using APIs.DTOs;
// // using APIs.Models;

// // namespace APIs.Repositories;

// // public interface IJobListingRepository
// // {
    
// //     Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync();
// //     Task<JobDetailResponse?> GetJobListingDetailAsync(Guid id);
// //     Task<bool> IsJobListingOpenAsync(Guid id);

    
// //     Task CreateJobListingAsync(JobListing listing);

// //     Task UpdateJobListingAsync(JobListing listing);

// //     Task CloseJobListingAsync(Guid id);

// //     Task DeleteJobListingAsync(JobListing job);

// // }

// using APIs.DTOs;
// using APIs.Models;

// namespace APIs.Repositories;

// public interface IJobListingRepository
// {
//     Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync();

//     Task<JobDetailResponse?> GetJobListingDetailAsync(Guid id);

//     Task<JobListing?> GetJobListingByIdAsync(Guid id);

//     Task<bool> JobListingExistsAsync(Guid id);

//     Task<bool> IsJobListingOpenAsync(Guid id);

//     Task<bool> DuplicateJobExistsAsync(string title, Guid companyId, string companyName);

// //creates a job listing along with it's company
//     Task CreateJobListingAsync( JobListing listing, Company? newCompany = null); 

//     Task UpdateJobListingAsync(JobListing listing, Company? newCompany = null);

//     Task CloseJobListingAsync(Guid id);

//     Task DeleteJobListingAsync(Guid id);
// }


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
}