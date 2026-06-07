// using APIs.Data;
// using APIs.DTOs;
// using APIs.Models;
// using Microsoft.EntityFrameworkCore;

// namespace APIs.Repositories;

// public class ApplicationRepository(CareerHubDbContext db) : IApplicationRepository
// {
//     public async Task<bool> HasApplicantAlreadyAppliedAsync(Guid jobListingId, Guid applicantId)
//     {
//         return await db.Applications
//             .AnyAsync(a => a.JobListingId == jobListingId && a.ApplicantId == applicantId);
//     }

//     public async Task<IEnumerable<ApplicationResponse>> GetApplicationsListAsync(Guid jobListingId)
//     {
//         return await db.Applications
//             .AsNoTracking()
//             .Where(a => a.JobListingId == jobListingId)
//             .Select(a => new ApplicationResponse(
//                 JobListingId: a.JobListingId,
//                 ApplicantId: a.ApplicantId,
//                 ApplicantName: $"{a.Applicant.FirstName} {a.Applicant.LastName}",
//                 SubmittedAt: a.SubmittedAt,
//                 Status: a.Status.ToString()
//             ))
//             .ToListAsync();
//     }

//     public async Task<IEnumerable<ApplicationResponse>> GetApplicationsByApplicantIdAsync(Guid applicantId)
//     {
//         return await db.Applications
//             .AsNoTracking()
//             .Where(a => a.ApplicantId == applicantId)
//             .Select(a => new ApplicationResponse(
//                 JobListingId: a.JobListingId,
//                 ApplicantId: a.ApplicantId,
//                 ApplicantName: $"{a.Applicant.FirstName} {a.Applicant.LastName}",
//                 SubmittedAt: a.SubmittedAt,
//                 Status: a.Status.ToString()
//             ))
//             .ToListAsync();
//     }

//     public async Task CreateApplicationAsync(Application application, Applicant? newApplicant)
//     {
//         if (newApplicant is not null)
//             db.Applicants.Add(newApplicant);

//         db.Applications.Add(application);
//         await db.SaveChangesAsync();
//     }

//     public async Task UpdateApplicationStatusAsync(Guid jobListingId, Guid applicantId, ApplicationStatus newStatus)
//     {
//         var application = await db.Applications
//             .FirstOrDefaultAsync(a =>
//                 a.JobListingId == jobListingId &&
//                 a.ApplicantId == applicantId);

//         if (application is null)
//             return;

//         application.Status = newStatus;
//         await db.SaveChangesAsync();
//     }
// }


using APIs.Data;
using APIs.DTOs;
using APIs.Models;
using Microsoft.EntityFrameworkCore;

namespace APIs.Repositories;

public class ApplicationRepository(CareerHubDbContext db)
    : IApplicationRepository
{
    
    public async Task<bool> HasApplicantAlreadyAppliedAsync(
        Guid jobListingId,
        Guid applicantId)
    {
        return await db.Applications
            .AnyAsync(a =>
                a.JobListingId == jobListingId &&
                a.ApplicantId == applicantId);
    }

    public async Task<Applicant?> GetApplicantByEmailAsync(
        string email)
    {
        return await db.Applicants
            .FirstOrDefaultAsync(a =>
                a.Email.ToLower() ==
                email.ToLower());
    }

    public async Task<IEnumerable<ApplicationResponse>>
        GetApplicationsListAsync(Guid jobListingId)
    {
        return await db.Applications
            .AsNoTracking()
            .Where(a => a.JobListingId == jobListingId)
            .Select(a => new ApplicationResponse(
                a.JobListingId,
                a.ApplicantId,
                $"{a.Applicant.FirstName} {a.Applicant.LastName}",
                a.SubmittedAt,
                a.Status.ToString()
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<ApplicationResponse>>
        GetApplicationsByApplicantIdAsync(Guid applicantId)
    {
        return await db.Applications
            .AsNoTracking()
            .Where(a => a.ApplicantId == applicantId)
            .Select(a => new ApplicationResponse(
                a.JobListingId,
                a.ApplicantId,
                $"{a.Applicant.FirstName} {a.Applicant.LastName}",
                a.SubmittedAt,
                a.Status.ToString()
            ))
            .ToListAsync();
    }

    public async Task CreateApplicationAsync(
        Application application,
        Applicant? newApplicant)
    {
        if (newApplicant is not null)
            db.Applicants.Add(newApplicant);

        db.Applications.Add(application);

        await db.SaveChangesAsync();
    }

    public async Task UpdateApplicationStatusAsync(
        Guid jobListingId,
        Guid applicantId,
        ApplicationStatus newStatus)
    {
        var application = await db.Applications
            .FirstOrDefaultAsync(a =>
                a.JobListingId == jobListingId &&
                a.ApplicantId == applicantId);

        if (application is null)
            return;

        application.Status = newStatus;

        await db.SaveChangesAsync();
    }
}