using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APIs.DTOs;
using APIs.Services;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;

namespace APIs.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/applications")]
public class ApplicationsController(IApplicationService applicationService) : ControllerBase
{
[Authorize(Roles = "Employer")]
[HttpGet("{id}")] // GET /api/v1/applications/{id}
public async Task<IActionResult> GetApplicationByIdAsync(string id)
{
    // Split the composite route tracking token: "jobListingId_applicantId"
    var keys = id.Split('_');
    if (keys.Length != 2 || !Guid.TryParse(keys[0], out var jobListingId) || !Guid.TryParse(keys[1], out var applicantId))
    {
        return BadRequest("Invalid composite tracking ID format. Use: {jobListingId}_{applicantId}");
    }
    // Fetch the single application resource using your existing service layer logic
    var applications = await applicationService.GetApplicationsForListingAsync(jobListingId);
    var application = applications.FirstOrDefault(a => a.ApplicantId == applicantId);
    
    if (application is null) return NotFound("Application not found.");
    // Compute ETag using the tracking ID string and the string representation of the Status
    string rawEtag = $"{id}_{application.Status}";
    string eTag = $"\"{rawEtag}\""; 

    // Check If-None-Match header
    if (Request.Headers.IfNoneMatch == eTag)
    {
        return StatusCode(StatusCodes.Status304NotModified);  
    }
    Response.Headers.ETag = eTag;
    return Ok(application);
}


    [Authorize(Roles = "Employer")]
    [HttpGet("listing/{jobListingId:guid}")]
    public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetApplicationsForListingAsync(Guid jobListingId)
    {
        var applications = await applicationService.GetApplicationsForListingAsync(jobListingId);
        return Ok(applications);
    }

    [Authorize(Roles = "Applicant")]
    [HttpGet("applicant/{applicantId:guid}")]
    public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetApplicationsByApplicantAsync(Guid applicantId)
    {
        var applications = await applicationService.GetApplicationsByApplicantAsync(applicantId);
        return Ok(applications);
    }

    //[Authorize(Roles = "Applicant")]
    [HttpPost("apply")]
    [EnableRateLimiting("apply")]
    public async Task<ActionResult<ApplicationResponse>> SubmitApplicationAsync([FromBody] CreateApplicationRequest request)
    {
        var application = await applicationService.SubmitApplicationAsync(request);
        return CreatedAtAction(nameof(GetApplicationsForListingAsync), new { jobListingId = application.JobListingId }, application);
    }

    [Authorize(Roles = "Employer")]
    [EndpointSummary("Update application status")]
    [EndpointDescription("Updates the current status of a job application. " +
                         "Legal transitions: Submitted -> UnderReview, UnderReview -> Shortlisted, Shortlisted -> Offered or Rejected. " +
                         "Illegal status transitions (Rejected -> Offered, Offered -> Shortlisted, Rejected -> Submitted) returns a 400 Bad Request.")]
    [HttpPatch("{jobListingId:guid}/{applicantId:guid}/status")]
    public async Task<IActionResult> PatchStatusAsync(
        [FromRoute] Guid jobListingId,
        [FromRoute] Guid applicantId,
        [FromBody] UpdateApplicationStatusRequest request)
        => Ok(await applicationService.PatchStatusAsync(jobListingId, applicantId, request.Status));


    [Authorize(Roles = "Employer")]
    [HttpPut("{jobListingId:guid}/{applicantId:guid}")]
    public async Task<ActionResult<ApplicationResponse>> UpdateApplicationStatusAsync(
        Guid jobListingId,
        Guid applicantId,
        [FromBody] UpdateApplicationRequest request)
    {
        var application = await applicationService.UpdateApplicationStatusAsync(jobListingId, applicantId, request);
        return Ok(application);
    }

    [Authorize(Roles = "Applicant")]
    [HttpDelete("{jobListingId:guid}/{applicantId:guid}/withdraw")]
    public async Task<IActionResult> WithdrawApplicationAsync(Guid jobListingId, Guid applicantId)
    {
        await applicationService.WithdrawApplicationAsync(jobListingId, applicantId);
        return NoContent();
    }
}