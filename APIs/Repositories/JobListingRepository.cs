// using APIs.Data;
// using APIs.DTOs;
// using APIs.Models;
// using Microsoft.EntityFrameworkCore;

// namespace APIs.Repositories;

// public class JobListingRepository(CareerHubDbContext db) : IJobListingRepository
// {
//     public async Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync()
//     {
//         var jobs = await db.JobListings
//             .AsNoTracking()
//             .Where(j => j.IsActive && j.ClosingDate > DateTime.UtcNow)
//             .Select(j => new {
//                 j.Id,
//                 j.Title,
//                 CompanyName = j.Company.CompanyName,
//                 j.Location,
//                 SalaryDisplay = j.SalaryMin.HasValue && j.SalaryMax.HasValue
//                     ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
//                     : j.SalaryMin.HasValue
//                         ? $"From R{j.SalaryMin:N0}/month"
//                         : "Salary not specified",
//                 j.ClosingDate,
//                 ApplicationCount = j.Applications.Count()
//             })
//             .ToListAsync();

//         return jobs.Select(aj => new JobListResponse(
//             aj.Id,
//             aj.Title,
//             aj.CompanyName,
//             aj.Location,
//             aj.SalaryDisplay,
//             aj.ApplicationCount,
//             aj.ClosingDate
//         ));
//     }

//     public async Task<JobDetailResponse?> GetJobListingDetailAsync(Guid id)
//     {
//         var job = await db.JobListings
//             .AsNoTracking()
//             .Include(j => j.Company)
//             .Include(j => j.Applications)
//                 .ThenInclude(a => a.Applicant)
//             .Where(j => j.Id == id)
//             .FirstOrDefaultAsync();

//         if (job is null)
//             return null;

//         return new JobDetailResponse(
//             Id: job.Id,
//             Title: job.Title,
//             CompanyId: job.CompanyId,
//             CompanyName: job.Company.CompanyName,
//             Location: job.Location,
//             Description: job.Description,
//             SalaryDisplay: job.SalaryMin.HasValue && job.SalaryMax.HasValue
//                 ? $"R{job.SalaryMin:N0} – R{job.SalaryMax:N0}/month"
//                 : job.SalaryMin.HasValue
//                     ? $"From R{job.SalaryMin:N0}/month"
//                     : "Salary not specified",
//             PostedAt: job.PostedAt,
//             ClosingDate: job.ClosingDate,
//             IsActive: job.IsActive,
//             Applications: job.Applications
//                 .Select(a => new ApplicationSummary(
//                     ApplicantName: $"{a.Applicant.FirstName} {a.Applicant.LastName}",
//                     SubmittedAt: a.SubmittedAt,
//                     Status: a.Status.ToString()
//                 )).ToList()
//         );
//     }

//     public async Task<bool> IsJobListingOpenAsync(Guid id)
//     {
//         return await db.JobListings
//             .AnyAsync(j => j.Id == id && j.IsActive && j.ClosingDate > DateTime.UtcNow);
//     }

//     public async Task CreateJobListingAsync(JobListing listing)
//     {
//         db.JobListings.Add(listing);
//         await db.SaveChangesAsync();
//     }

//     public async Task UpdateJobListingAsync(JobListing listing)
//     {
//         db.JobListings.Update(listing);
//         await db.SaveChangesAsync();
//     }

//     public async Task CloseJobListingAsync(Guid id)
//     {
//         var listing = await db.JobListings.FindAsync(id);

//         if (listing is null)
//             return;

//         listing.IsActive = false;
//         await db.SaveChangesAsync();
//     }
// }

using APIs.Data;
using APIs.DTOs;
using APIs.Models;
using Microsoft.EntityFrameworkCore;

namespace APIs.Repositories;

public class JobListingRepository(CareerHubDbContext db)
    : IJobListingRepository
{
    public async Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync()
    {
        var jobs = await db.JobListings
            .AsNoTracking()
            .Where(j => j.IsActive && j.ClosingDate > DateTime.UtcNow)
            .Select(j => new
            {
                j.Id,
                j.Title,
                CompanyName = j.Company.CompanyName,
                j.Location,
                SalaryDisplay =
                    j.SalaryMin.HasValue && j.SalaryMax.HasValue
                    ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
                    : j.SalaryMin.HasValue
                        ? $"From R{j.SalaryMin:N0}/month"
                        : "Salary not specified",
                j.ClosingDate,
                ApplicationCount = j.Applications.Count()
            })
            .ToListAsync();

        return jobs.Select(j => new JobListResponse(
            j.Id,
            j.Title,
            j.CompanyName,
            j.Location,
            j.SalaryDisplay,
            j.ApplicationCount,
            j.ClosingDate
        ));
    }

    public async Task<JobDetailResponse?> GetJobListingDetailAsync(Guid id)
    {
        var job = await db.JobListings
            .AsNoTracking()
            .Include(j => j.Company)
            .Include(j => j.Applications)
                .ThenInclude(a => a.Applicant)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null)
            return null;

        return new JobDetailResponse(
            Id: job.Id,
            Title: job.Title,
            CompanyId: job.CompanyId,
            CompanyName: job.Company.CompanyName,
            Location: job.Location,
            Description: job.Description,
            SalaryDisplay:
                job.SalaryMin.HasValue && job.SalaryMax.HasValue
                ? $"R{job.SalaryMin:N0} – R{job.SalaryMax:N0}/month"
                : job.SalaryMin.HasValue
                    ? $"From R{job.SalaryMin:N0}/month"
                    : "Salary not specified",
            PostedAt: job.PostedAt,
            ClosingDate: job.ClosingDate,
            IsActive: job.IsActive,
            Applications: job.Applications
                .Select(a => new ApplicationSummary(
                    ApplicantName:
                        $"{a.Applicant.FirstName} {a.Applicant.LastName}",
                    SubmittedAt: a.SubmittedAt,
                    Status: a.Status.ToString()
                ))
                .ToList()
        );
    }

    public async Task<JobListing?> GetJobListingByIdAsync(Guid id)
    {
        return await db.JobListings
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<bool> JobListingExistsAsync(Guid id)
    {
        return await db.JobListings
            .AnyAsync(j => j.Id == id);
    }

    public async Task<bool> IsJobListingOpenAsync(Guid id)
    {
        return await db.JobListings
            .AnyAsync(j =>
                j.Id == id &&
                j.IsActive &&
                j.ClosingDate > DateTime.UtcNow);
    }

    public async Task<bool> DuplicateJobExistsAsync(
        string title,
        Guid companyId, string companyName)
    {
        return await db.JobListings
            .AnyAsync(j =>
                j.Title.ToLower() == title.ToLower() &&
                j.CompanyId == companyId);
    }

    public async Task<Company?> GetCompanyByNameAsync(
        string companyName)
    {
        return await db.Companies
            .FirstOrDefaultAsync(c =>
                c.CompanyName.ToLower() ==
                companyName.ToLower());
    }

    public async Task CreateJobListingAsync(
        JobListing jobListing,
        Company? newCompany = null)
    {
        if (newCompany is not null)
            db.Companies.Add(newCompany);

        db.JobListings.Add(jobListing);

        await db.SaveChangesAsync();
    }

    public async Task UpdateJobListingAsync(
        JobListing jobListing,
        Company? newCompany = null)
    {
        if (newCompany is not null)
            db.Companies.Add(newCompany);

        db.JobListings.Update(jobListing);

        await db.SaveChangesAsync();
    }

    public async Task CloseJobListingAsync(Guid id)
    {
        var jobListing = await db.JobListings
            .FindAsync(id);

        if (jobListing is null)
            return;

        jobListing.IsActive = false;

        await db.SaveChangesAsync();
    }

    public async Task DeleteJobListingAsync(Guid id)
    {
        var jobListing = await db.JobListings
            .FindAsync(id);

        if (jobListing is null)
            return;

        db.JobListings.Remove(jobListing);

        await db.SaveChangesAsync();
    }
}