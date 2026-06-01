using Microsoft.AspNetCore.Mvc;
using APIs.Models;
using APIs.Data;
using System.Runtime.InteropServices;
using APIs.DTOs;
using APIs.Exceptions;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(JobListingStore jobService) : ControllerBase
{

    // Returns all available job listings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobListing>>> GetJobsAsync()
    {
        // Calls the service to Get all jobs
        var jobs = await jobService.GetAllJobsAsync();

        // Return HTTP 200 OK status with the job data
        return Ok(jobs);
    }


    // Returns a single job listing by ID
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobListing>> GetJobByIdAsync(Guid id)
    {
        // Call the service to Get a job by ID
        var job = await jobService.GetJobByIdAsync(id);

        // Return HTTP 404 status if the job does not exist
        if (job is null)
        {
              throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
        }

        // Return HTTP 200 OK status with the job data
        return Ok(job);
    }

    [HttpPost]
public async Task<ActionResult<JobResponse>> CreateJobAsync([FromBody] CreateJobRequest request)
{
    await Task.CompletedTask;

    // 1. IDEMPOTENCY
    // Prevent a duplicate addition of a Job if the client submits the same form twice.
    bool isDuplicate = JobListingStore.Jobs.Any(j =>
        j.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase) &&
        j.Company.Equals(request.Company, StringComparison.OrdinalIgnoreCase)
    );

    if (isDuplicate)
    {
        throw new DuplicateJobException(request.Title, request.Company);
    }

    // 2. Map DTO → Domain Model
    var newJob = new JobListing
    {
        Id          = Guid.NewGuid(),
        Title       = request.Title,
        Company     = request.Company,
        Location    = request.Location,
        Description = request.Description,
        Type        = request.Type,
        SalaryMin   = request.SalaryMin,
        SalaryMax   = request.SalaryMax,
        PostedAt    = DateTime.UtcNow,
        IsActive    = true
    };

    // 3. Save the Job listing
    JobListingStore.Jobs.Add(newJob);

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
    Id:            job.Id,
    Title:         job.Title,
    Company:       job.Company,
    Location:      job.Location,
    Description:   job.Description,
    Type:          job.Type,
    SalaryMin:     job.SalaryMin,
    SalaryMax:     job.SalaryMax,
    PostedAt:      job.PostedAt,
    IsActive:      job.IsActive,
    SalaryDisplay: salaryDisplay

    );
} 

 // Updates existing Job fields
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<JobResponse>> UpdateJobAsync(Guid id, [FromBody] UpdateJobRequest request)
    {
    await Task.CompletedTask;

        // Find the existing job
        var existing = JobListingStore.Jobs.FirstOrDefault(j => j.Id == id);

        // Return 404 if it doesn't exist
        if (existing is null)
        {
            throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
        }

        // Replace editable fields — PostedAt and IsActive are preserved
        existing.Title       = request.Title;
        existing.Company     = request.Company;
        existing.Location    = request.Location;
        existing.Description = request.Description;
        existing.Type        = request.Type;
        existing.SalaryMin   = request.SalaryMin;
        existing.SalaryMax   = request.SalaryMax;

        // Map updated job → Response DTO
        var response = MapToResponse(existing);

        // Return 200 OK with updated job in body
        return Ok(response);
    }

    // Deletes a job listing by ID
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteJobAsync(Guid id)
    {
    await Task.CompletedTask;

        // Find the existing job
        var existing = JobListingStore.Jobs.FirstOrDefault(j => j.Id == id);

        // Return 404 if it doesn't exist
        if (existing is null)
        {
            throw new JobNotFoundException(id); //Throws new exception for Jobs Not Found
        }

        // Remove the job
        JobListingStore.Jobs.Remove(existing);

        // Return 204 No Content — job is gone, nothing to return
        return NoContent();
    }

}