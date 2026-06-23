

// using APIs.Data;
// using APIs.DTOs;
// using APIs.Exceptions;
// using APIs.Models;
// using APIs.Services;
// using Microsoft.EntityFrameworkCore;

// namespace APIs.Repositories;

// public class ApplicationRepository(CareerHubDbContext db)
//     : IApplicationRepository
// {
    
//     public async Task<bool> HasApplicantAlreadyAppliedAsync(
//         Guid jobListingId,
//         Guid applicantId)
//     {
//         return await db.Applications
//             .AnyAsync(a =>
//                 a.JobListingId == jobListingId &&
//                 a.ApplicantId == applicantId);
//     }

//     public async Task<Applicant?> GetApplicantByEmailAsync(
//         string email)
//     {
//         return await db.Applicants
//             .FirstOrDefaultAsync(a =>
//                 a.Email.ToLower() ==
//                 email.ToLower());
//     }

//     public async Task<IEnumerable<ApplicationResponse>> GetApplicationsListAsync(Guid jobListingId)
//     {
//         return await db.Applications
//             .AsNoTracking()
//             .Include(a => a.JobListing)
//             .Include(a => a.Applicant)
//             .Where(a => a.JobListingId == jobListingId)
            
//             .Select(a => new ApplicationResponse(
//                 a.JobListingId, // Mapping JobListingId as the DTO Id for now
//                 a.JobListingId,
//                 a.ApplicantId,
//                 a.JobListing.Title,
//                 $"{a.Applicant.FirstName} {a.Applicant.LastName}",
//                 a.SubmittedAt,
//                 a.Status.ToString()
//             ))
//             .ToListAsync();
//     }

//     public async Task<IEnumerable<ApplicationResponse>>
//         GetApplicationsByApplicantIdAsync(Guid applicantId)
//     {
//         return await db.Applications
//             .AsNoTracking() 
//             .Include(a => a.JobListing)
//             .Include(a => a.Applicant)
//             .Where(a => a.ApplicantId == applicantId)
           
//             .Select(a => new ApplicationResponse(
//                 a.JobListingId,
//                 a.JobListingId,
//                 a.ApplicantId,
//                 a.JobListing.Title,
//                 $"{a.Applicant.FirstName} {a.Applicant.LastName}",
//                 a.SubmittedAt,
//                 a.Status.ToString()
//             ))
//             .ToListAsync();
//     }

//     public async Task CreateApplicationAsync(
//         Application application,
//         Applicant? newApplicant)
//     {
//         if (newApplicant is not null)
//             db.Applicants.Add(newApplicant);

//         db.Applications.Add(application);

//         await db.SaveChangesAsync();
//     }

//     public async Task UpdateApplicationStatusAsync(
//         Guid jobListingId,
//         Guid applicantId,
//         ApplicationStatus newStatus)
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

//     public async Task WithdrawApplicationAsync(Guid jobListingId, Guid applicantId)
// {
//     var application = await db.Applications
//         .FirstOrDefaultAsync(a =>
//             a.JobListingId == jobListingId &&
//             a.ApplicantId == applicantId);

//     if (application is null)
//         return;

//     db.Applications.Remove(application);
//     await db.SaveChangesAsync();
// }

// public async Task<ApplicationResponse> PatchStatusAsync(
//     Guid jobListingId,
//     Guid applicantId,
//     ApplicationStatus status)
// {
//     var application = await db.Applications
//         .Include(a => a.Applicant)
//         .Include(a => a.JobListing)
//         .FirstOrDefaultAsync(a =>
//             a.JobListingId == jobListingId &&
//             a.ApplicantId == applicantId)
//         ?? throw new ApplicationNotFoundException(jobListingId, applicantId);

//     if (!ApplicationService.IsValidTransition(application.Status, status))
//         throw new InvalidStatusTransitionException(application.Status, status);

//     application.Status = status;
//     await db.SaveChangesAsync();

//     return new ApplicationResponse(
//         Id: application.JobListingId,
//         JobListingId: application.JobListingId,
//         ApplicantId: application.ApplicantId,
//         JobTitle: application.JobListing.Title,
//         ApplicantName: $"{application.Applicant.FirstName} {application.Applicant.LastName}",
//         SubmittedAt: application.SubmittedAt,
//         Status: application.Status.ToString()
//     );
// }
// }

using APIs.Data;
using APIs.DTOs;
using APIs.Exceptions;
using APIs.Models;
using APIs.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Repositories;

public class ApplicationRepository(CareerHubDbContext db) : IApplicationRepository
{
    public async Task<bool> HasApplicantAlreadyAppliedAsync(Guid jobListingId, Guid applicantId)
    {
        return await db.Applications.AnyAsync(a => a.JobListingId == jobListingId && a.ApplicantId == applicantId);
    }

    public async Task<Applicant?> GetApplicantByEmailAsync(string email)
    {
        return await db.Applicants.FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<ApplicationResponse>> GetApplicationsListAsync(Guid jobListingId)
    {
        return await db.Applications
            .AsNoTracking()
            .Include(a => a.JobListing)
            .Include(a => a.Applicant)
            .Where(a => a.JobListingId == jobListingId)
            .Select(a => new ApplicationResponse(
                a.JobListingId,
                a.JobListingId,
                a.ApplicantId,
                a.JobListing.Title,
                a.FullName ?? $"{a.Applicant.FirstName} {a.Applicant.LastName}",
                a.Email ?? a.Applicant.Email,
                a.Phone,
                a.YearsOfExperience,
                a.CoverLetter ?? string.Empty,
                a.LinkedInUrl,
                a.AvailableImmediately,
                a.NoticePeriodWeeks,
                a.SubmittedAt,
                a.Status
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<ApplicationResponse>> GetApplicationsByApplicantIdAsync(Guid applicantId)
    {
        return await db.Applications
            .AsNoTracking() 
            .Include(a => a.JobListing)
            .Include(a => a.Applicant)
            .Where(a => a.ApplicantId == applicantId)
            .Select(a => new ApplicationResponse(
                a.JobListingId,
                a.JobListingId,
                a.ApplicantId,
                a.JobListing.Title,
                a.FullName ?? $"{a.Applicant.FirstName} {a.Applicant.LastName}",
                a.Email ?? a.Applicant.Email,
                a.Phone,
                a.YearsOfExperience,
                a.CoverLetter ?? string.Empty,
                a.LinkedInUrl,
                a.AvailableImmediately,
                a.NoticePeriodWeeks,
                a.SubmittedAt,
                a.Status
            ))
            .ToListAsync();
    }

    public async Task CreateApplicationAsync(Application application, Applicant? newApplicant)
    {
        if (newApplicant is not null)
            db.Applicants.Add(newApplicant);

        db.Applications.Add(application);
        await db.SaveChangesAsync();
    }

    public async Task UpdateApplicationStatusAsync(Guid jobListingId, Guid applicantId, ApplicationStatus newStatus)
    {
        var application = await db.Applications.FirstOrDefaultAsync(a => a.JobListingId == jobListingId && a.ApplicantId == applicantId);
        if (application is null) return;

        application.Status = newStatus;
        await db.SaveChangesAsync();
    }

    public async Task WithdrawApplicationAsync(Guid jobListingId, Guid applicantId)
    {
        var application = await db.Applications.FirstOrDefaultAsync(a => a.JobListingId == jobListingId && a.ApplicantId == applicantId);
        if (application is null) return;

        db.Applications.Remove(application);
        await db.SaveChangesAsync();
    }

    public async Task<ApplicationResponse> PatchStatusAsync(Guid jobListingId, Guid applicantId, ApplicationStatus status)
    {
        var application = await db.Applications
            .Include(a => a.Applicant)
            .Include(a => a.JobListing)
            .FirstOrDefaultAsync(a => a.JobListingId == jobListingId && a.ApplicantId == applicantId)
            ?? throw new ApplicationNotFoundException(jobListingId, applicantId);

        if (!ApplicationService.IsValidTransition(application.Status, status))
            throw new InvalidStatusTransitionException(application.Status, status);

        application.Status = status;
        await db.SaveChangesAsync();

        return new ApplicationResponse(
            Id: application.JobListingId,
            JobListingId: application.JobListingId,
            ApplicantId: application.ApplicantId,
            JobTitle: application.JobListing.Title,
            ApplicantName: application.FullName ?? $"{application.Applicant.FirstName} {application.Applicant.LastName}",
            Email: application.Email ?? application.Applicant.Email,
            Phone: application.Phone,
            YearsOfExperience: application.YearsOfExperience,
            CoverLetter: application.CoverLetter ?? string.Empty,
            LinkedInUrl: application.LinkedInUrl,
            AvailableImmediately: application.AvailableImmediately,
            NoticePeriodWeeks: application.NoticePeriodWeeks,
            SubmittedAt: application.SubmittedAt,
            Status: application.Status
        );
    }
}