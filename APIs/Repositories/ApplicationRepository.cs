

using APIs.Data;
using APIs.DTOs;
using APIs.Exceptions;
using APIs.Models;
using APIs.Services;
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

    public async Task<IEnumerable<ApplicationResponse>> GetApplicationsListAsync(Guid jobListingId)
    {
        return await db.Applications
            .AsNoTracking()
            .Include(a => a.JobListing)
            .Where(a => a.JobListingId == jobListingId)
            
            .Select(a => new ApplicationResponse(
                JobListingId: a.JobListingId,
    ApplicantId: a.ApplicantId,
    JobTitle: a.JobListing.Title,
    ApplicantName: $"{a.Applicant.FirstName} {a.Applicant.LastName}",
    SubmittedAt: a.SubmittedAt,
    Status: a.Status.ToString(),
    Id: a.JobListingId
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<ApplicationResponse>>
        GetApplicationsByApplicantIdAsync(Guid applicantId)
    {
        return await db.Applications
            .AsNoTracking() 
            .Include(a => a.JobListing)
            .Where(a => a.ApplicantId == applicantId)
           
            .Select(a => new ApplicationResponse(
                JobListingId: a.JobListingId,
    ApplicantId: a.ApplicantId,
    JobTitle: a.JobListing.Title,
    ApplicantName: $"{a.Applicant.FirstName} {a.Applicant.LastName}",
    SubmittedAt: a.SubmittedAt,
    Status: a.Status.ToString(),
    Id: a.JobListingId
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

    public async Task WithdrawApplicationAsync(Guid jobListingId, Guid applicantId)
{
    var application = await db.Applications
        .FirstOrDefaultAsync(a =>
            a.JobListingId == jobListingId &&
            a.ApplicantId == applicantId);

    if (application is null)
        return;

    db.Applications.Remove(application);
    await db.SaveChangesAsync();
}

public async Task<ApplicationResponse> PatchStatusAsync(
    Guid jobListingId,
    Guid applicantId,
    ApplicationStatus status)
{
    var application = await db.Applications
        .Include(a => a.Applicant)
        .Include(a => a.JobListing)
        .FirstOrDefaultAsync(a =>
            a.JobListingId == jobListingId &&
            a.ApplicantId == applicantId)
        ?? throw new ApplicationNotFoundException(jobListingId, applicantId);

    if (!ApplicationService.IsValidTransition(application.Status, status))
        throw new InvalidStatusTransitionException(application.Status, status);

    application.Status = status;
    await db.SaveChangesAsync();

    return new ApplicationResponse(
        JobListingId: application.JobListingId,
        ApplicantId: application.ApplicantId,
        JobTitle: application.JobListing.Title,
        ApplicantName: $"{application.Applicant.FirstName} {application.Applicant.LastName}",
        SubmittedAt: application.SubmittedAt,
        Status: application.Status.ToString(),
        Id: application.JobListingId
    );
}
}