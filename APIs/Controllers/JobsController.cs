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
    public async Task<ActionResult<IEnumerable<JobListing>>> GetJobsAsync()
    {
        // Calls the database to Get all jobs
        var jobs = await db.JobListings.ToListAsync();
        // Return HTTP 200 OK status with the job data
        return Ok(jobs);
    }


    // Returns a single job listing by ID
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobListing>> GetJobByIdAsync(Guid id)
    {
        // Call the database to Get a job by ID
        var job = await db.JobListings.FindAsync(id);

        // Return HTTP 404 status if the job does not exist
        if (job is null)
        {
            throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
        }

        // Return HTTP 200 OK status with the job data
        return Ok(job);
    }


    [Authorize(Roles = "Employer")]
    [HttpPost]
    public async Task<ActionResult<JobResponse>> CreateJobAsync([FromBody] CreateJobRequest request)
    {
        //await Task.CompletedTask;

        // 1. IDEMPOTENCY
        // Prevent a duplicate addition of a Job if the client submits the same form twice.
        bool isDuplicate = await db.JobListings.AnyAsync(j =>
            j.Title.ToLower() == request.Title.ToLower() &&
 j.CompanyId == request.CompanyId        );

        if (isDuplicate)
        {
            throw new DuplicateJobException(request.Title, request.CompanyId.ToString());
        }

        // 2. Map DTO → Domain Model
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

        // 3. Save the Job listing
        db.JobListings.Add(newJob);

        await db.SaveChangesAsync();

        // 4. Map Domain Model → Response DTO
        var response = MapToResponse(newJob);

        // 5. Return 201 Created + Location header
        return CreatedAtAction(nameof(GetJobByIdAsync), new { id = newJob.Id }, response);
    }

    private static JobResponse MapToResponse(JobListing job)
    {
        string salaryDisplay;

        if (job.SalaryMin.HasValue && job.SalaryMax.HasValue)
            salaryDisplay = $"R{job.SalaryMin:N0} – R{job.SalaryMax:N0}/month";
        else if (job.SalaryMin.HasValue)
            salaryDisplay = $"From R{job.SalaryMin:N0}/month";
        else
            salaryDisplay = "Salary not specified";

        return new JobResponse
        (
        Id: job.Id,
        Title: job.Title,
        CompanyId: job.CompanyId,
        Location: job.Location,
        Description: job.Description,
        Type: job.Type,
        SalaryMin: job.SalaryMin,
        SalaryMax: job.SalaryMax,
        PostedAt: job.PostedAt,
        IsActive: job.IsActive,
        SalaryDisplay: salaryDisplay

        );
    }

    // Updates existing Job fields
    [Authorize(Roles = "Employer")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<JobResponse>> UpdateJobAsync(Guid id, [FromBody] UpdateJobRequest request)
    {
        //await Task.CompletedTask;

        // Find the existing job
        //var existing = JobListingStore.Jobs.FirstOrDefault(j => j.Id == id);
        var existing = await db.JobListings.FindAsync(id);

        // Return 404 if it doesn't exist
        if (existing is null)
        {
            throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
        }

        // Replace editable fields — PostedAt and IsActive are preserved
        existing.Title = request.Title;
        existing.CompanyId = request.CompanyId;
        existing.Location = request.Location;
        existing.Description = request.Description;
        existing.Type = request.Type;
        existing.SalaryMin = request.SalaryMin;
        existing.SalaryMax = request.SalaryMax;

        //Saves and updates the database
        await db.SaveChangesAsync();

        // Map updated job → Response DTO
        var response = MapToResponse(existing);

        // Return 200 OK with updated job in body
        return Ok(response);
    }

    // Deletes a job listing by ID
    [Authorize(Roles = "Employer")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteJobAsync(Guid id)
    {
        await Task.CompletedTask;

        // Find the existing job
        //var existing = JobListingStore.Jobs.FirstOrDefault(j => j.Id == id);
        var existing = await db.JobListings.FindAsync(id);


        // Return 404 if it doesn't exist
        if (existing is null)
        {
            throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
        }

        // Remove the job
        db.JobListings.Remove(existing);
        //Saves and updates the database
        await db.SaveChangesAsync();
        // Return 204 No Content — job is gone, nothing to return
        return NoContent();
    }

}