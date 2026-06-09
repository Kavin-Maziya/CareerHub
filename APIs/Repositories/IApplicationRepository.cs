using APIs.DTOs;
using APIs.Models;

namespace APIs.Repositories;

public interface IApplicationRepository
{
    
    
    Task<bool> HasApplicantAlreadyAppliedAsync(Guid jobListingId, Guid applicantId);
    Task<IEnumerable<ApplicationResponse>> GetApplicationsListAsync(Guid jobListingId);
    Task<IEnumerable<ApplicationResponse>> GetApplicationsByApplicantIdAsync(Guid applicantId);
    Task<Applicant?> GetApplicantByEmailAsync(string email);

    Task CreateApplicationAsync(Application application, Applicant? newApplicant);
    Task UpdateApplicationStatusAsync(Guid jobListingId, Guid applicantId, ApplicationStatus newStatus);
    
    Task WithdrawApplicationAsync(Guid jobListingId, Guid applicantId);

    Task<ApplicationResponse> PatchStatusAsync(Guid jobListingId, Guid applicantId, ApplicationStatus status);
}