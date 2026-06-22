using APIs.Data;
using APIs.DTOs;
using APIs.Exceptions;
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
                        j.PostedAt,
                        j.SalaryMin.HasValue && j.SalaryMax.HasValue
                            ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
                            : j.SalaryMin.HasValue
                                ? $"From R{j.SalaryMin:N0}/month"
                                : "Salary not specified",
                        j.ClosingDate,
                        j.Applications.Count(),
                        j.IsActive,
                        j.EmploymentType  
                    ))
        );

    private static readonly Func<CareerHubDbContext, Guid, Task<bool>>
        _jobListingExistsCompiled = EF.CompileAsyncQuery(
            (CareerHubDbContext ctx, Guid id) =>
                ctx.JobListings.Any(j => j.Id == id)
        );

    public async Task<PagedResponse<JobListResponse>> GetAllListingsPagedAsync(int page, int pageSize, JobListingFilterQuery filter)
    {
        var query = db.JobListings
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Location))
            query = query.Where(j => j.Location.ToLower().Contains(filter.Location.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.EmploymentType))
        {
            Enum.TryParse<EmploymentType>(filter.EmploymentType, ignoreCase: true, out var employmentType);
            query = query.Where(j => j.EmploymentType == employmentType);
        }

        if (filter.SalaryMin.HasValue)
            query = query.Where(j => j.SalaryMin >= filter.SalaryMin.Value);

        if (filter.SalaryMax.HasValue)
            query = query.Where(j => j.SalaryMax <= filter.SalaryMax.Value);

        if (filter.CompanyId.HasValue)
            query = query.Where(j => j.CompanyId == filter.CompanyId.Value);

        var totalCount = await query.CountAsync();

        query = (filter.Sort.ToLower(), filter.Dir.ToLower()) switch
        {
            ("salarymin", "asc")  => query.OrderBy(j => j.SalaryMin),
            ("salarymin", _)      => query.OrderByDescending(j => j.SalaryMin),
            ("salarymax", "desc") => query.OrderByDescending(j => j.SalaryMax),
            ("salarymax", _)      => query.OrderBy(j => j.SalaryMax),
            ("title", "desc")     => query.OrderByDescending(j => j.Title),
            ("title", _)          => query.OrderBy(j => j.Title),
            _                     => query.OrderByDescending(j => j.PostedAt)
        };

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JobListResponse(
                j.Id,
                j.Title,
                j.Company.CompanyName,
                j.Location,
                j.PostedAt,
                j.SalaryMin.HasValue && j.SalaryMax.HasValue
                    ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
                    : j.SalaryMin.HasValue
                        ? $"From R{j.SalaryMin:N0}/month"
                        : "Salary not specified",
                j.ClosingDate,
                j.Applications.Count(),
                j.IsActive,
                j.EmploymentType 
            ))
            .ToListAsync();

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PagedResponse<JobListResponse>
        {
            Data = data,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

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
                j.PostedAt,
                j.SalaryMin.HasValue && j.SalaryMax.HasValue
                    ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
                    : j.SalaryMin.HasValue
                        ? $"From R{j.SalaryMin:N0}/month"
                        : "Salary not specified",
                j.ClosingDate,
                j.Applications.Count(),
                j.IsActive,
                j.EmploymentType  
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

    public async Task<PagedResponse<JobListResponse>> GetActiveListingsPagedAsync(int page, int pageSize, JobListingFilterQuery filter)
    {
        var query = db.JobListings
            .AsNoTracking()
            .Where(j => j.IsActive && j.ClosingDate > DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(filter.Location))
            query = query.Where(j => j.Location.ToLower().Contains(filter.Location.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.EmploymentType))
        {
            Enum.TryParse<EmploymentType>(filter.EmploymentType, ignoreCase: true, out var employmentType);
            query = query.Where(j => j.EmploymentType == employmentType);
        }

        if (filter.SalaryMin.HasValue)
            query = query.Where(j => j.SalaryMin >= filter.SalaryMin.Value);

        if (filter.SalaryMax.HasValue)
            query = query.Where(j => j.SalaryMax <= filter.SalaryMax.Value);

        if (filter.CompanyId.HasValue)
            query = query.Where(j => j.CompanyId == filter.CompanyId.Value);

        var totalCount = await query.CountAsync();

        query = (filter.Sort.ToLower(), filter.Dir.ToLower()) switch
        {
            ("salarymin", "asc")  => query.OrderBy(j => j.SalaryMin),
            ("salarymin", _)      => query.OrderByDescending(j => j.SalaryMin),
            ("salarymax", "desc") => query.OrderByDescending(j => j.SalaryMax),
            ("salarymax", _)      => query.OrderBy(j => j.SalaryMax),
            ("title", "desc")     => query.OrderByDescending(j => j.Title),
            ("title", _)          => query.OrderBy(j => j.Title),
            _                     => query.OrderByDescending(j => j.PostedAt)
        };

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JobListResponse(
                j.Id,
                j.Title,
                j.Company.CompanyName,
                j.Location,
                j.PostedAt,
                j.SalaryMin.HasValue && j.SalaryMax.HasValue
                    ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
                    : j.SalaryMin.HasValue
                        ? $"From R{j.SalaryMin:N0}/month"
                        : "Salary not specified",
                j.ClosingDate,
                j.Applications.Count(),
                j.IsActive,
                j.EmploymentType  
            ))
            .ToListAsync();

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PagedResponse<JobListResponse>
        {
            Data = data,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<JobListResponse> PatchAsync(Guid id, UpdateJobListingRequest request)
    {
        var listing = await db.JobListings
            .Include(j => j.Company)
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id)
            ?? throw new JobNotFoundException(id);

        if (request.Title is not null)
            listing.Title = request.Title;

        if (request.Description is not null)
            listing.Description = request.Description;

        if (request.Location is not null)
            listing.Location = request.Location;

        if (request.EmploymentType is not null &&
            Enum.TryParse<EmploymentType>(request.EmploymentType, ignoreCase: true, out var employmentType))
            listing.EmploymentType = employmentType;

        if (request.SalaryMin is not null || request.SalaryMax is not null)
        {
            var newMin = request.SalaryMin ?? listing.SalaryMin;
            var newMax = request.SalaryMax ?? listing.SalaryMax;

            if (newMin.HasValue && newMax.HasValue && newMin > newMax)
                throw new InvalidSalaryRangeException();

            if (request.SalaryMin is not null)
                listing.SalaryMin = request.SalaryMin;

            if (request.SalaryMax is not null)
                listing.SalaryMax = request.SalaryMax;
        }

        if (request.ExpiresAt is not null)
        {
            if (request.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidExpiryDateException();

            listing.ClosingDate = request.ExpiresAt.Value;
        }

        await db.SaveChangesAsync();

        return new JobListResponse(
            Id: listing.Id,
            Title: listing.Title,
            CompanyName: listing.Company.CompanyName,
            Location: listing.Location,
            PostedAt: listing.PostedAt,
            SalaryDisplay: listing.SalaryMin.HasValue && listing.SalaryMax.HasValue
                ? $"R{listing.SalaryMin:N0} – R{listing.SalaryMax:N0}/month"
                : listing.SalaryMin.HasValue
                    ? $"From R{listing.SalaryMin:N0}/month"
                    : "Salary not specified",
            ClosingDate: listing.ClosingDate,
            ApplicationCount: listing.Applications.Count(),
            IsActive: listing.IsActive,
            EmploymentType: listing.EmploymentType  
        );
    }
}