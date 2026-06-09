using Microsoft.AspNetCore.Mvc;
using APIs.DTOs;
using APIs.Services;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;

namespace APIs.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/jobs")]
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
    public async Task<IActionResult> GetJobByIdAsync(Guid id)
    {
        var job = await jobListingService.GetJobListingDetailAsync(id);
        if (job is null) return NotFound();

        string rawEtag = $"{id}_{job.PostedAt.Ticks}_{job.SalaryDisplay}";
        string eTag = $"\"{rawEtag}\"";

    if (Request.Headers.IfNoneMatch == eTag)
    return StatusCode(StatusCodes.Status304NotModified);

    Response.Headers.ETag = eTag;
    return Ok(job);

    
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<JobListResponse>>> GetJobsAsync(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? location = null,
    [FromQuery] string? employmentType = null,
    [FromQuery] decimal? salaryMin = null,
    [FromQuery] decimal? salaryMax = null,
    [FromQuery] Guid? companyId = null,
    [FromQuery] string sort = "postedAt",
    [FromQuery] string dir = "desc")
    {
        var filter = new JobListingFilterQuery
        {
            Location = location,
            EmploymentType = employmentType,
            SalaryMin = salaryMin,
            SalaryMax = salaryMax,
            CompanyId = companyId,
            Sort = sort,
            Dir = dir
        };

        var result = await jobListingService.GetActiveListingsPagedAsync(page, pageSize, filter);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }
    [Authorize(Roles = "Employer")]
    [HttpPatch("{id:guid}")]
     public async Task<ActionResult<JobListResponse>> PatchJobAsync(Guid id, [FromBody] UpdateJobListingRequest request)
         => Ok(await jobListingService.PatchAsync(id, request));

    // Full-text search endpoint
    [AllowAnonymous]
    [HttpGet("search")]
    [EnableRateLimiting("search")]
    public async Task<ActionResult<IEnumerable<JobListResponse>>> SearchJobsAsync([FromQuery] string q)
        => Ok(await jobListingService.SearchAsync(q));

    // Application statistics endpoint — controller is one line as required
    [Authorize(Roles = "Employer")]
    [HttpGet("stats")]
    public async Task<ActionResult<IEnumerable<JobListingStatsResponse>>> GetStatsAsync([FromQuery] Guid companyId)
        => Ok(await jobListingService.GetApplicationStatsAsync(companyId));

    [Authorize(Roles = "Employer")]
    [EnableRateLimiting("post-listing")]
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
