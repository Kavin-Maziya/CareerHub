using APIs.Data;
using APIs.DTOs;
using APIs.Models;
using Microsoft.EntityFrameworkCore;

namespace APIs.Repositories;

public class JobListingRepository(CareerHubDbContext db) : IJobListingRepository
{
   
    private static readonly Func<CareerHubDbContext, IAsyncEnumerable<JobListResponse>>
        _getActiveListingsCompiled = EF.CompileAsyncQuery(
            (CareerHubDbContext ctx) =>
                ctx.JobListings
                    .AsNoTracking()
                    .Where(j => j.IsActive && j.ClosingDate > DateTime.UtcNow)
                    .Select(j => new JobListResponse(
                        j.Id,
                        j.Title,
                        j.Company.CompanyName,
                        j.Location,
                        j.SalaryMin.HasValue && j.SalaryMax.HasValue
                            ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
                            : j.SalaryMin.HasValue
                                ? $"From R{j.SalaryMin:N0}/month"
                                : "Salary not specified",
                        j.Applications.Count(),
                        j.ClosingDate
                    ))
        );

    private static readonly Func<CareerHubDbContext, Guid, Task<bool>>
        _jobListingExistsCompiled = EF.CompileAsyncQuery(
            (CareerHubDbContext ctx, Guid id) =>
                ctx.JobListings.Any(j => j.Id == id)
        );


    public async Task<IEnumerable<JobListResponse>> GetActiveJobListingsAsync()
    {
        var results = new List<JobListResponse>();
        await foreach (var item in _getActiveListingsCompiled(db))
            results.Add(item);
        return results;
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
                    ApplicantName: $"{a.Applicant.FirstName} {a.Applicant.LastName}",
                    SubmittedAt: a.SubmittedAt,
                    Status: a.Status.ToString()
                ))
                .ToList()
        );
    }

    public async Task<JobListing?> GetJobListingByIdAsync(Guid id)
    {
        return await db.JobListings
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<bool> JobListingExistsAsync(Guid id)
    {
        return await _jobListingExistsCompiled(db, id);
    }

    public async Task<bool> IsJobListingOpenAsync(Guid id)
    {
        return await db.JobListings
            .AnyAsync(j =>
                j.Id == id &&
                j.IsActive &&
                j.ClosingDate > DateTime.UtcNow);
    }

    public async Task<bool> DuplicateJobExistsAsync(string title, string companyName)
    {
        return await db.JobListings
            .AnyAsync(j =>
                j.Title.ToLower() == title.ToLower() &&
                j.Company.CompanyName.ToLower() == companyName.ToLower());
    }

    public async Task CreateJobListingAsync(JobListing listing, string companyName, string industry)
    {
        var company = await db.Companies
            .FirstOrDefaultAsync(c => c.CompanyName.ToLower() == companyName.ToLower());

        if (company is null)
        {
            company = new Company
            {
                CompanyId = Guid.NewGuid(),
                CompanyName = companyName,
                Industry = industry
            };

            db.Companies.Add(company);
        }

        listing.CompanyId = company.CompanyId;
        db.JobListings.Add(listing);
        await db.SaveChangesAsync();
    }

    public async Task UpdateJobListingAsync(JobListing listing, string companyName, string industry)
    {
        var company = await db.Companies
            .FirstOrDefaultAsync(c => c.CompanyName.ToLower() == companyName.ToLower());

        if (company is null)
        {
            company = new Company
            {
                CompanyId = Guid.NewGuid(),
                CompanyName = companyName,
                Industry = industry
            };

            db.Companies.Add(company);
        }

        listing.CompanyId = company.CompanyId;
        db.JobListings.Update(listing);
        await db.SaveChangesAsync();
    }

    public async Task CloseJobListingAsync(Guid id)
    {
        var jobListing = await db.JobListings.FindAsync(id);

        if (jobListing is null)
            return;

        jobListing.IsActive = false;
        await db.SaveChangesAsync();
    }

    public async Task DeleteJobListingAsync(Guid id)
    {
        var jobListing = await db.JobListings.FindAsync(id);

        if (jobListing is null)
            return;

        db.JobListings.Remove(jobListing);
        await db.SaveChangesAsync();
    }

    // Text Search ─────────────────────────────────────────────

    public async Task<IEnumerable<JobListResponse>> SearchAsync(string searchTerm)
    {
        var results = await db.JobListings
            .AsNoTracking()
            .Where(j =>
                j.IsActive &&
                j.ClosingDate > DateTime.UtcNow &&
                j.SearchVector!.Matches(EF.Functions.ToTsQuery("english", searchTerm)))
            .Select(j => new JobListResponse(
                j.Id,
                j.Title,
                j.Company.CompanyName,
                j.Location,
                j.SalaryMin.HasValue && j.SalaryMax.HasValue
                    ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
                    : j.SalaryMin.HasValue
                        ? $"From R{j.SalaryMin:N0}/month"
                        : "Salary not specified",
                j.Applications.Count(),
                j.ClosingDate
            ))
            .ToListAsync();

        return results;
    }

    
    public async Task<IEnumerable<JobListingStatsResponse>> GetApplicationStatsAsync(Guid companyId)
    {
        
        var results = await db.Database
            .SqlQuery<JobListingStatsResponse>($@"
                SELECT
                    j.""Id""           AS ""JobListingId"",
                    j.""Title""        AS ""Title"",
                    COUNT(a.*)         AS ""TotalApplications"",
                    COUNT(*) FILTER (WHERE a.""Status"" = 0) AS ""Submitted"",
                    COUNT(*) FILTER (WHERE a.""Status"" = 1) AS ""UnderReview"",
                    COUNT(*) FILTER (WHERE a.""Status"" = 2) AS ""Shortlisted"",
                    COUNT(*) FILTER (WHERE a.""Status"" = 3) AS ""Offered"",
                    COUNT(*) FILTER (WHERE a.""Status"" = 4) AS ""Rejected"",
                    RANK() OVER (ORDER BY COUNT(a.*) DESC) AS ""Rank""
                FROM job_listings j
                LEFT JOIN applications a ON a.""JobListingId"" = j.""Id""
                WHERE j.""CompanyId"" = {companyId}
                  AND j.""IsActive"" = true
                GROUP BY j.""Id"", j.""Title""
                ORDER BY ""Rank""
            ")
            .ToListAsync();

        return results;
    }
}
