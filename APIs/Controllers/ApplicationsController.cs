using Microsoft.AspNetCore.Mvc;
using APIs.DTOs;
using APIs.Services;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace APIs.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/[controller]")]
public class ApplicationsController(IApplicationService applicationService) : ControllerBase
{
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

    [Authorize(Roles = "Applicant")]
    [HttpPost]
    public async Task<ActionResult<ApplicationResponse>> SubmitApplicationAsync([FromBody] CreateApplicationRequest request)
    {
        var application = await applicationService.SubmitApplicationAsync(request);
        return CreatedAtAction(nameof(GetApplicationsForListingAsync), new { jobListingId = application.JobListingId }, application);
    }

    [Authorize(Roles = "Employer")]
    [HttpPatch("{jobListingId:guid}/applicant/{applicantId:guid}/status")]
    public async Task<IActionResult> PatchStatusAsync(
    Guid jobListingId,
    Guid applicantId,
    [FromBody] PatchApplicationStatusRequest request)
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