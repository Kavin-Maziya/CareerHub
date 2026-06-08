using Microsoft.AspNetCore.Mvc;
using APIs.DTOs;
using APIs.Services;
using Microsoft.AspNetCore.Authorization;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(IJobListingService jobListingService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobListResponse>>> GetJobsAsync()
    {
        var jobs = await jobListingService.GetActiveJobListingsAsync();
        return Ok(jobs);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobDetailResponse>> GetJobByIdAsync(Guid id)
    {
        var job = await jobListingService.GetJobListingDetailAsync(id);
        return Ok(job);
    }

    // Part 5: Full-text search endpoint — controller is one line as required
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<JobListResponse>>> SearchJobsAsync([FromQuery] string q)
        => Ok(await jobListingService.SearchAsync(q));

    // Part 8: Application statistics endpoint — controller is one line as required
    [Authorize(Roles = "Employer")]
    [HttpGet("stats")]
    public async Task<ActionResult<IEnumerable<JobListingStatsResponse>>> GetStatsAsync([FromQuery] Guid companyId)
        => Ok(await jobListingService.GetApplicationStatsAsync(companyId));

    [Authorize(Roles = "Employer")]
    [HttpPost]
    public async Task<ActionResult<JobListResponse>> CreateJobAsync([FromBody] CreateJobRequest request)
    {
        var job = await jobListingService.CreateJobListingAsync(request);
        return CreatedAtAction(nameof(GetJobByIdAsync), new { id = job.Id }, job);
    }

    [Authorize(Roles = "Employer")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<JobListResponse>> UpdateJobAsync(Guid id, [FromBody] UpdateJobRequest request)
    {
        var job = await jobListingService.UpdateJobListingAsync(id, request);
        return Ok(job);
    }

    [Authorize(Roles = "Employer")]
    [HttpDelete("{id:guid}/close")]
    public async Task<IActionResult> CloseJobAsync(Guid id)
    {
        await jobListingService.CloseJobListingAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Employer")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteJobAsync(Guid id)
    {
        await jobListingService.DeleteJobListingAsync(id);
        return NoContent();
    }
}
