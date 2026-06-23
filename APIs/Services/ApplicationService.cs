using APIs.DTOs;
using APIs.Exceptions;
using APIs.Models;
using APIs.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Services;

public class ApplicationService(
    IApplicationRepository applicationRepository,
    IJobListingRepository jobListingRepository) : IApplicationService
{
    // Application Status Transition Validation Matrix
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
        bool isOpen = await jobListingRepository.IsJobListingOpenAsync(request.JobListingId);
        if (!isOpen)
            throw new ListingClosedException(request.JobListingId);

        var applicant = await applicationRepository.GetApplicantByEmailAsync(request.Email);
        Applicant? newApplicant = null;

        if (applicant is null)
        {
            var nameParts = request.FullName.Split(' ', 2);
            string firstName = nameParts[0];
            string lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            newApplicant = new Applicant
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = request.Email
            };
            applicant = newApplicant;
        }

        bool alreadyApplied = await applicationRepository.HasApplicantAlreadyAppliedAsync(request.JobListingId, applicant.Id);
        if (alreadyApplied)
            throw new DuplicateApplicationException(request.JobListingId, applicant.Id);

        var application = new Application
        {
            JobListingId = request.JobListingId,
            ApplicantId = applicant.Id,
            SubmittedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Submitted,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            YearsOfExperience = request.YearsOfExperience,
            CoverLetter = request.CoverLetter,
            LinkedInUrl = request.LinkedInUrl,
            AvailableImmediately = request.AvailableImmediately,
            NoticePeriodWeeks = request.NoticePeriodWeeks
        };

        await applicationRepository.CreateApplicationAsync(application, newApplicant);

        return new ApplicationResponse(
            Id: application.JobListingId,
            JobListingId: application.JobListingId,
            ApplicantId: applicant.Id,
            JobTitle: string.Empty,
            ApplicantName: request.FullName,
            Email: application.Email,
            Phone: application.Phone,
            YearsOfExperience: application.YearsOfExperience,
            CoverLetter: application.CoverLetter,
            LinkedInUrl: application.LinkedInUrl,
            AvailableImmediately: application.AvailableImmediately,
            NoticePeriodWeeks: application.NoticePeriodWeeks,
            SubmittedAt: application.SubmittedAt,
            Status: application.Status // FIX: Pass the Enum object directly instead of .ToString()
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

    // 1. Parse the incoming string from the request into a valid Enum token
    if (!Enum.TryParse<ApplicationStatus>(request.Status, ignoreCase: true, out var newStatus))
        throw new InvalidStatusTransitionException(request.Status);

    // 2. FIX: Pass both 'application.Status' and 'newStatus' directly as Enums 
    if (!IsValidTransition(application.Status, newStatus))
        throw new InvalidStatusTransitionException(application.Status, newStatus);

    await applicationRepository.UpdateApplicationStatusAsync(jobListingId, applicantId, newStatus);

    return application with { Status = newStatus };
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

        if (application.ApplicantId != applicantId)
            throw new UnauthorizedWithdrawalException(applicantId);

        await applicationRepository.WithdrawApplicationAsync(jobListingId, applicantId);
    }

    public async Task<ApplicationResponse> PatchStatusAsync(
        Guid jobListingId,
        Guid applicantId,
        ApplicationStatus status)
    {
        return await applicationRepository.PatchStatusAsync(jobListingId, applicantId, status);
    }
}