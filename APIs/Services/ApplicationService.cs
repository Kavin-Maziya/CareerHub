using APIs.DTOs;
using APIs.Exceptions;
using APIs.Models;
using APIs.Repositories;

namespace APIs.Services;

public class ApplicationService(
    IApplicationRepository applicationRepository,
    IJobListingRepository jobListingRepository) : IApplicationService
{
    //Application Status Transition
    private static readonly HashSet<(ApplicationStatus From, ApplicationStatus To)> _validTransitions =
    [
        (ApplicationStatus.Submitted, ApplicationStatus.UnderReview),
        (ApplicationStatus.UnderReview, ApplicationStatus.Shortlisted),
        (ApplicationStatus.UnderReview, ApplicationStatus.Rejected),
        (ApplicationStatus.Shortlisted, ApplicationStatus.Offered),
        (ApplicationStatus.Shortlisted, ApplicationStatus.Rejected)
    ];

    public static bool IsValidTransition(ApplicationStatus from, ApplicationStatus to)
        => _validTransitions.Contains((from, to));

    public async Task<IEnumerable<ApplicationResponse>> GetApplicationsForListingAsync(Guid jobListingId)
    {
        return await applicationRepository.GetApplicationsListAsync(jobListingId);
    }

    public async Task<IEnumerable<ApplicationResponse>> GetApplicationsByApplicantAsync(Guid applicantId)
    {
        return await applicationRepository.GetApplicationsByApplicantIdAsync(applicantId);
    }

    public async Task<ApplicationResponse> SubmitApplicationAsync(CreateApplicationRequest request)
    {
        // Check listing exists and is open
        bool isOpen = await jobListingRepository.IsJobListingOpenAsync(request.JobListingId);

        if (!isOpen)
            throw new ListingClosedException(request.JobListingId);

        // Look up applicant by email, create if not found
        var applicant = await applicationRepository.GetApplicantByEmailAsync(request.Email);

        Applicant? newApplicant = null;

        if (applicant is null)
        {
            newApplicant = new Applicant
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email            
            };

            applicant = newApplicant;
        }

        // Check for duplicate application
        bool alreadyApplied = await applicationRepository.HasApplicantAlreadyAppliedAsync(
            request.JobListingId, applicant.Id);

        if (alreadyApplied)
            throw new DuplicateApplicationException(request.JobListingId, applicant.Id);

        var application = new Application
        {
            JobListingId = request.JobListingId,
            ApplicantId = applicant.Id,
            SubmittedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Submitted
        };

        await applicationRepository.CreateApplicationAsync(application, newApplicant);

        return new ApplicationResponse(
            JobListingId: application.JobListingId,
            ApplicantId: applicant.Id,
            ApplicantName: $"{applicant.FirstName} {applicant.LastName}",
            SubmittedAt: application.SubmittedAt,
            Status: application.Status.ToString()
        );
    }

    public async Task<ApplicationResponse> UpdateApplicationStatusAsync(
        Guid jobListingId,
        Guid applicantId,
        UpdateApplicationRequest request)
    {
        var applications = await applicationRepository.GetApplicationsListAsync(jobListingId);

        ApplicationResponse? application = null;

        foreach (var item in applications)
        {
            if (item.ApplicantId == applicantId)
            {
                application = item;
                break;
            }
        }

        if (application is null)
            throw new ApplicationNotFoundException(jobListingId, applicantId);

        if (!Enum.TryParse<ApplicationStatus>(request.Status, ignoreCase: true, out var newStatus))
            throw new InvalidStatusTransitionException(request.Status);

        if (!Enum.TryParse<ApplicationStatus>(application.Status, ignoreCase: true, out var currentStatus))
            throw new InvalidStatusTransitionException(application.Status);

        if (!IsValidTransition(currentStatus, newStatus))
            throw new InvalidStatusTransitionException(currentStatus, newStatus);

        await applicationRepository.UpdateApplicationStatusAsync(jobListingId, applicantId, newStatus);

        return new ApplicationResponse(
            JobListingId: jobListingId,
            ApplicantId: applicantId,
            ApplicantName: application.ApplicantName,
            SubmittedAt: application.SubmittedAt,
            Status: newStatus.ToString()
        );
    }
    public async Task WithdrawApplicationAsync(Guid jobListingId, Guid applicantId)
    {
        var applications = await applicationRepository.GetApplicationsListAsync(jobListingId);

        ApplicationResponse? application = null;

        foreach (var item in applications)
        {
            if (item.ApplicantId == applicantId)
            {
                application = item;
                break;
            }
        }

    if (application is null)
        throw new ApplicationNotFoundException(jobListingId, applicantId);

    // Only the applicant who submitted can withdraw
    if (application.ApplicantId != applicantId)
        throw new UnauthorizedWithdrawalException(applicantId);

    await applicationRepository.WithdrawApplicationAsync(jobListingId, applicantId);
}
}