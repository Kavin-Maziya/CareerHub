using APIs.DTOs;
using APIs.Models;

namespace APIs.Services;

public interface IApplicationService
{
    Task<IEnumerable<ApplicationResponse>> GetApplicationsForListingAsync(Guid jobListingId);
    Task<IEnumerable<ApplicationResponse>> GetApplicationsByApplicantAsync(Guid applicantId);
    Task<ApplicationResponse> SubmitApplicationAsync(CreateApplicationRequest request);
    Task<ApplicationResponse> UpdateApplicationStatusAsync(Guid jobListingId, Guid applicantId, UpdateApplicationRequest request);
    Task WithdrawApplicationAsync(Guid jobListingId, Guid applicantId);
Task<ApplicationResponse> PatchStatusAsync(Guid jobListingId, Guid applicantId, ApplicationStatus status);

}