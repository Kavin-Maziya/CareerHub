using APIs.Data;
using APIs.DTOs;
using APIs.Models;
using Microsoft.EntityFrameworkCore;

namespace APIs.Repositories;

public class JobListingRepository(CareerHubDbContext db) : IJobListingRepository
{
    // ── Part 6: Compiled Queries ─────────────────────────────────────────────
    //
    // GetActiveJobListingsAsync is a hot path: called on every page load of the
    // public job board. With 1,000 active daily users making ~3 page loads per
    // session, this method executes ~3,000 times per day (~2–4 times per minute
    // at peak). EF Core re-parses the LINQ expression tree on every call without
    // compilation. Compiling it once at startup eliminates that overhead entirely.
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

    // JobListingExistsAsync is a hot path: called on every employer write operation
    // (update, close, delete) to verify the listing exists before acting. In a
    // deployment where employers manage listings frequently, this runs on every
    // mutating request — potentially dozens of times per minute across all employers.
    // Compiling avoids repeated expression tree compilation for a trivially simple query.
    private static readonly Func<CareerHubDbContext, Guid, Task<bool>>
        _jobListingExistsCompiled = EF.CompileAsyncQuery(
            (CareerHubDbContext ctx, Guid id) =>
                ctx.JobListings.Any(j => j.Id == id)
        );

    // ── Public Methods ───────────────────────────────────────────────────────

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
        // Delegates to the compiled query — public signature unchanged.
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

    // ── Part 5: Full-Text Search ─────────────────────────────────────────────

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

    // ── Part 8: Raw SQL with RANK() window function ───────────────────────────
    //
    // EF Core's LINQ translator cannot express RANK() OVER (...) window functions
    // or COUNT(*) FILTER (WHERE ...) conditional aggregation. Attempting to write
    // this in LINQ would require loading all application rows into memory and
    // performing grouping and ranking in C#, which is impractical at scale.
    // FromSql with a parameterised query is the correct solution here.
    public async Task<IEnumerable<JobListingStatsResponse>> GetApplicationStatsAsync(Guid companyId)
    {
        // String interpolation inside SqlQuery<T> is safe because EF Core
        // intercepts the interpolated string and converts each {variable} into
        // a proper parameterised SQL parameter (@p0, @p1, etc.) — identical to
        // calling SqlQuery with explicit SqlParameter objects. The value is never
        // concatenated into the SQL string itself.
        //
        // Using string.Format() or + concatenation BEFORE passing to SqlQuery<T>
        // is unsafe because the interpolation happens in C# first, producing a
        // plain string with the value baked in — EF Core receives a string with
        // no parameter markers and cannot protect against SQL injection.
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
