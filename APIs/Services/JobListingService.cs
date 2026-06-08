using APIs.DTOs;
using APIs.Exceptions;
using APIs.Models;
using APIs.Repositories;

namespace APIs.Services;

public class JobListingService(IJobListingRepository jobListingRepository) : IJobListingService
{
    public async Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync()
    {
        return await jobListingRepository.GetActiveJobListingsAsync();
    }

    public async Task<JobDetailResponse> GetJobListingDetailAsync(Guid id)
    {
        var listing = await jobListingRepository.GetJobListingDetailAsync(id);

        if (listing is null)
            throw new JobNotFoundException(id);

        return listing;
    }

    public async Task<bool> JobListingExistsAsync(Guid id)
    {
        return await jobListingRepository.JobListingExistsAsync(id);
    }

    public async Task<bool> IsJobListingOpenAsync(Guid id)
    {
        return await jobListingRepository.IsJobListingOpenAsync(id);
    }

    public async Task<bool> DuplicateJobExistsAsync(string title, string companyName)
    {
        return await jobListingRepository.DuplicateJobExistsAsync(title, companyName);
    }

    public async Task<JobListResponse> CreateJobListingAsync(CreateJobRequest request)
    {
        bool isDuplicate = await jobListingRepository.DuplicateJobExistsAsync(
            request.Title,
            request.CompanyName);

        if (isDuplicate)
            throw new DuplicateJobException(request.Title, request.CompanyName);

        var listing = new JobListing
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Location = request.Location,
            Description = request.Description,
            Type = request.Type,
            ClosingDate = request.ClosingDate,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            PostedAt = DateTime.UtcNow,
            IsActive = true
        };

        await jobListingRepository.CreateJobListingAsync(listing, request.CompanyName, request.Industry ?? string.Empty);

        return new JobListResponse(
            Id: listing.Id,
            Title: listing.Title,
            CompanyName: request.CompanyName,
            Location: listing.Location,
            SalaryDisplay: MapSalaryDisplay(listing.SalaryMin, listing.SalaryMax),
            ApplicationCount: 0,
            ClosingDate: listing.ClosingDate
        );
    }

    public async Task<JobListResponse> UpdateJobListingAsync(Guid id, UpdateJobRequest request)
    {
        var existing = await jobListingRepository.GetJobListingByIdAsync(id);

        if (existing is null)
            throw new JobNotFoundException(id);

        if (!existing.IsActive || existing.ClosingDate <= DateTime.UtcNow)
            throw new ListingClosedException(id);

        if (!existing.Company.CompanyName.Equals(request.CompanyName, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedCompanyException(id);

        existing.Title = request.Title;
        existing.Location = request.Location;
        existing.Description = request.Description;
        existing.Type = request.Type;
        existing.ClosingDate = request.ClosingDate;
        existing.SalaryMin = request.SalaryMin;
        existing.SalaryMax = request.SalaryMax;

        await jobListingRepository.UpdateJobListingAsync(existing, request.CompanyName, request.Industry ?? string.Empty);

        return new JobListResponse(
            Id: existing.Id,
            Title: existing.Title,
            CompanyName: request.CompanyName,
            Location: existing.Location,
            SalaryDisplay: MapSalaryDisplay(existing.SalaryMin, existing.SalaryMax),
            ApplicationCount: 0,
            ClosingDate: existing.ClosingDate
        );
    }

    public async Task CloseJobListingAsync(Guid id)
    {
        bool exists = await jobListingRepository.JobListingExistsAsync(id);

        if (!exists)
            throw new JobNotFoundException(id);

        await jobListingRepository.CloseJobListingAsync(id);
    }

    public async Task DeleteJobListingAsync(Guid id)
    {
        bool exists = await jobListingRepository.JobListingExistsAsync(id);

        if (!exists)
            throw new JobNotFoundException(id);

        await jobListingRepository.DeleteJobListingAsync(id);
    }

    // Part 5: Full-text search delegates straight to the repository
    public async Task<IEnumerable<JobListResponse>> SearchAsync(string searchTerm)
    {
        return await jobListingRepository.SearchAsync(searchTerm);
    }

    // Part 8: Application stats delegates straight to the repository
    public async Task<IEnumerable<JobListingStatsResponse>> GetApplicationStatsAsync(Guid companyId)
    {
        return await jobListingRepository.GetApplicationStatsAsync(companyId);
    }

    private static string MapSalaryDisplay(decimal? salaryMin, decimal? salaryMax)
    {
        if (salaryMin.HasValue && salaryMax.HasValue)
            return $"R{salaryMin:N0} R{salaryMax:N0}/month";

        if (salaryMin.HasValue)
            return $"From R{salaryMin:N0}/month";

        return "Salary not specified";
    }
}
