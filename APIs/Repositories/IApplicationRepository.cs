using APIs.DTOs;
using APIs.Models;

namespace APIs.Repositories;

public interface IApplicationRepository
{
    
    
    Task<bool> HasApplicantAlreadyAppliedAsync(Guid jobListingId, Guid applicantId);
    Task<IEnumerable<ApplicationResponse>> GetApplicationsListAsync(Guid jobListingId);
    Task<IEnumerable<ApplicationResponse>> GetApplicationsByApplicantIdAsync(Guid applicantId);

    Task CreateApplicationAsync(Application application, Applicant? newApplicant);
    Task UpdateApplicationStatusAsync(Guid jobListingId, Guid applicantId, ApplicationStatus newStatus);
}