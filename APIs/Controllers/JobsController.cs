using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using APIs.Models;
using APIs.Data;
using APIs.DTOs;
using APIs.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(CareerHubDbContext db) : ControllerBase
{
    // Returns all available job listings
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobListResponse>>> GetJobsAsync()
    {
        var jobs = await db.JobListings
            .AsNoTracking()
            .Select(j => new JobListResponse(
                Id: j.Id,
                Title: j.Title,
                CompanyName: j.Company.CompanyName,
                Location: j.Location,
                SalaryDisplay: j.SalaryMin.HasValue && j.SalaryMax.HasValue
                    ? $"R{j.SalaryMin:N0} – R{j.SalaryMax:N0}/month"
                    : j.SalaryMin.HasValue
                        ? $"From R{j.SalaryMin:N0}/month"
                        : "Salary not specified",
                ApplicationCount: j.Applications.Count()
            ))
            .ToListAsync();

        return Ok(jobs);
    }

    // Returns a single job listing by ID
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobDetailResponse>> GetJobByIdAsync(Guid id)
    {
        var jobEntity = await db.JobListings
            .AsNoTracking()
            .Include(j => j.Company)
            .Include(j => j.Applications)
                .ThenInclude(a => a.Applicant)
            .Where(j => j.Id == id)
            .FirstOrDefaultAsync();

        if (jobEntity is null)
        {
            throw new JobNotFoundException(id);
        }

        var job = MapToDetailResponse(jobEntity);

        return Ok(job);
    }

    [Authorize(Roles = "Employer")]
    [HttpPost]
    public async Task<ActionResult<JobListResponse>> CreateJobAsync([FromBody] CreateJobRequest request)
    {
        // Prevent duplicate job listings
        bool isDuplicate = await db.JobListings.AnyAsync(j =>
            j.Title.ToLower() == request.Title.ToLower() &&
            j.CompanyId == request.CompanyId
        );

        if (isDuplicate)
        {
            throw new DuplicateJobException(request.Title, request.CompanyId.ToString());
        }

        // Map DTO → Domain Model
        var newJob = new JobListing
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            CompanyId = request.CompanyId,
            Location = request.Location,
            Description = request.Description,
            Type = request.Type,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            PostedAt = DateTime.UtcNow,
            IsActive = true
        };

        db.JobListings.Add(newJob);
        await db.SaveChangesAsync();

        var response = MapToListResponse(newJob);

        return CreatedAtAction(nameof(GetJobByIdAsync), new { id = newJob.Id }, response);
    }

    [Authorize(Roles = "Employer")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<JobListResponse>> UpdateJobAsync(Guid id, [FromBody] UpdateJobRequest request)
    {
        var existing = await db.JobListings.FindAsync(id);

        if (existing is null)
        {
            throw new JobNotFoundException(id);
        }

        // Replace editable fields — PostedAt and IsActive are preserved
        existing.Title = request.Title;
        existing.CompanyId = request.CompanyId;
        existing.Location = request.Location;
        existing.Description = request.Description;
        existing.Type = request.Type;
        existing.SalaryMin = request.SalaryMin;
        existing.SalaryMax = request.SalaryMax;

        await db.SaveChangesAsync();

        var response = MapToListResponse(existing);

        return Ok(response);
    }

    [Authorize(Roles = "Employer")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteJobAsync(Guid id)
    {
        var existing = await db.JobListings.FindAsync(id);

        if (existing is null)
        {
            throw new JobNotFoundException(id);
        }

        db.JobListings.Remove(existing);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static JobListResponse MapToListResponse(JobListing job) => new(
        Id: job.Id,
        Title: job.Title,
        CompanyName: string.Empty, // company not loaded on write path
        Location: job.Location,
        SalaryDisplay: MapSalaryDisplay(job.SalaryMin, job.SalaryMax),
        ApplicationCount: 0
    );

    private static JobDetailResponse MapToDetailResponse(JobListing job) => new(
        Id: job.Id,
        Title: job.Title,
        CompanyId: job.CompanyId,
        CompanyName: job.Company.CompanyName,
        Location: job.Location,
        Description: job.Description,
        SalaryDisplay: MapSalaryDisplay(job.SalaryMin, job.SalaryMax),
        PostedAt: job.PostedAt,
        IsActive: job.IsActive,
        Applications: job.Applications
            .Select(a => new ApplicationSummary(
                ApplicantName: $"{a.Applicant.FirstName} {a.Applicant.LastName}",
                SubmittedAt: a.SubmittedAt,
                Status: a.Status.ToString()
            )).ToList()
    );

    private static string MapSalaryDisplay(decimal? salaryMin, decimal? salaryMax)
    {
        if (salaryMin.HasValue && salaryMax.HasValue)
            return $"R{salaryMin:N0} – R{salaryMax:N0}/month";

        if (salaryMin.HasValue)
            return $"From R{salaryMin:N0}/month";

        return "Salary not specified";
    }
}
























// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Mvc;
// using APIs.Models;
// using APIs.Data;
// using APIs.DTOs;
// using APIs.Exceptions;
// using Microsoft.AspNetCore.Authorization;

// namespace APIs.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class JobsController(CareerHubDbContext db) : ControllerBase
// {

//     // Returns all available job listings
//     [AllowAnonymous]

//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobsAsync()
//     {
//         // Calls the database to get all jobs
//         var jobs = await db.JobListings
//             .AsNoTracking()  // No change tracking for reads
//             .ToListAsync();

//         var response = jobs.Select(MapToResponse);

//         // Return HTTP 200 OK status with the job data
//         return Ok(response);
//     }


//     // Returns a single job listing by ID
//     [AllowAnonymous]
//     [HttpGet("{id:guid}")]
//     public async Task<ActionResult<JobListing>> GetJobByIdAsync(Guid id)
//     {
//         // Call the database to Get a job by ID
//         // var job = await db.JobListings.FindAsync(id);

//         // // Return HTTP 404 status if the job does not exist
//         // if (job is null)
//         // {
//         //     throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
//         // }

//         // // Return HTTP 200 OK status with the job data
//         // return Ok(job);


//         var jobEntity = await db.JobListings
//             .AsNoTracking()
//             .Include(j => j.Company)
//             .Include(j => j.Applications)
//                 .ThenInclude(a => a.Applicant)
//             .Where(j => j.Id == id)
//             .FirstOrDefaultAsync();

//         if (jobEntity is null)
//         {
//             throw new JobNotFoundException(id);
//         }

//         var job = MapToResponse(jobEntity);


//         return Ok(jobEntity);
//     }


//     [Authorize(Roles = "Employer")] // Only users with the "Employer" role can access this endpoint
//     [HttpPost]
//     public async Task<ActionResult<JobResponse>> CreateJobAsync([FromBody] CreateJobRequest request)
//     {
//         //await Task.CompletedTask;

//         // 1. IDEMPOTENCY
//         // Prevent a duplicate addition of a Job if the client submits the same form twice.
//         bool isDuplicate = await db.JobListings.AnyAsync(j =>
//             j.Title.ToLower() == request.Title.ToLower() &&
//             j.CompanyId == request.CompanyId
//         );

//         if (isDuplicate)
//         {
//             throw new DuplicateJobException(request.Title, request.CompanyId.ToString());
//         }

//         // 2. Map DTO → Domain Model
//         var newJob = new JobListing
//         {
//             Id = Guid.NewGuid(),
//             Title = request.Title,
//             CompanyId = request.CompanyId,
//             Location = request.Location,
//             Description = request.Description,
//             Type = request.Type,
//             SalaryMin = request.SalaryMin,
//             SalaryMax = request.SalaryMax,
//             PostedAt = DateTime.UtcNow,
//             IsActive = true
//         };

//         // 3. Save the Job listing
//         db.JobListings.Add(newJob);

//         await db.SaveChangesAsync();

//         // 4. Map Domain Model → Response DTO
//         var response = MapToResponse(newJob);

//         // 5. Return 201 Created + Location header
//         return CreatedAtAction(nameof(GetJobByIdAsync), new { id = newJob.Id }, response);
//     }

//     private static JobResponse MapToResponse(JobListing job)
//     {
//         string salaryDisplay;

//         if (job.SalaryMin.HasValue && job.SalaryMax.HasValue)
//             salaryDisplay = $"R{job.SalaryMin:N0} – R{job.SalaryMax:N0}/month";
//         else if (job.SalaryMin.HasValue)
//             salaryDisplay = $"From R{job.SalaryMin:N0}/month";
//         else
//             salaryDisplay = "Salary not specified";

//         return new JobResponse
//         (
//         Id: job.Id,
//         Title: job.Title,
//         CompanyId: job.CompanyId,
//         Location: job.Location,
//         Description: job.Description,
//         Type: job.Type,
//         SalaryMin: job.SalaryMin,
//         SalaryMax: job.SalaryMax,
//         PostedAt: job.PostedAt,
//         IsActive: job.IsActive,
//         SalaryDisplay: salaryDisplay

//         );
//     }

//     private static string MapSalaryDisplay(decimal? salaryMin, decimal? salaryMax)
//     {
//         if (salaryMin.HasValue && salaryMax.HasValue)
//             return $"R{salaryMin:N0} – R{salaryMax:N0}/month";

//         if (salaryMin.HasValue)
//             return $"From R{salaryMin:N0}/month";

//         return "Salary not specified";
//     }

//     // Updates existing Job fields
//     [Authorize(Roles = "Employer")]
//     [HttpPut("{id:guid}")]
//     public async Task<ActionResult<JobResponse>> UpdateJobAsync(Guid id, [FromBody] UpdateJobRequest request)
//     {
//         //await Task.CompletedTask;

//         // Find the existing job
//         //var existing = JobListingStore.Jobs.FirstOrDefault(j => j.Id == id);
//         var existing = await db.JobListings.FindAsync(id);

//         // Return 404 if it doesn't exist
//         if (existing is null)
//         {
//             throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
//         }

//         // Replace editable fields — PostedAt and IsActive are preserved
//         existing.Title = request.Title;
//         existing.CompanyId = request.CompanyId;
//         existing.Location = request.Location;
//         existing.Description = request.Description;
//         existing.Type = request.Type;
//         existing.SalaryMin = request.SalaryMin;
//         existing.SalaryMax = request.SalaryMax;

//         //Saves and updates the database
//         await db.SaveChangesAsync();

//         // Map updated job → Response DTO
//         var response = MapToResponse(existing);

//         // Return 200 OK with updated job in body
//         return Ok(response);
//     }

//     // Deletes a job listing by ID
//     [Authorize(Roles = "Employer")]
//     [HttpDelete("{id:guid}")]
//     public async Task<IActionResult> DeleteJobAsync(Guid id)
//     {
//         await Task.CompletedTask;

//         // Find the existing job
//         //var existing = JobListingStore.Jobs.FirstOrDefault(j => j.Id == id);
//         var existing = await db.JobListings.FindAsync(id);


//         // Return 404 if it doesn't exist
//         if (existing is null)
//         {
//             throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
//         }

//         // Remove the job
//         db.JobListings.Remove(existing);
//         //Saves and updates the database
//         await db.SaveChangesAsync();
//         // Return 204 No Content — job is gone, nothing to return
//         return NoContent();
//     }

// }