using Microsoft.AspNetCore.Mvc;
using APIs.Models;
using APIs.Data;
using System.Runtime.InteropServices;

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
            return NotFound();
        }

        // Return HTTP 200 OK status with the job data
        return Ok(job);
    }
}