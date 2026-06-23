// namespace APIs.Models;
// //Join table between Applicant and JobListing - represents a job application submitted by an applicant for a specific job listing
// public class Application // Represents a job application submitted by an applicant for a specific job listing
// {
//     //public Guid Id {get; set;}
//     public Guid JobListingId { get; set; }

//     public Guid ApplicantId { get; set; }

//     public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

//     public ApplicationStatus Status { get; set; }

//     public JobListing JobListing { get; set; } = null!;

//     public Applicant Applicant { get; set; } = null!;
// }

using System;

namespace APIs.Models;

public class Application 
{
    // Composite or foreign keys linking entities
    public Guid JobListingId { get; set; }
    public Guid ApplicantId { get; set; }

    // Historical Point-in-time snapshot details submitted for this application instance
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public int YearsOfExperience { get; set; }
    public string CoverLetter { get; set; } = null!;
    public string? LinkedInUrl { get; set; }
    public bool AvailableImmediately { get; set; }
    public int NoticePeriodWeeks { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public ApplicationStatus Status { get; set; }

    // EF Core Navigation Properties
    public JobListing JobListing { get; set; } = null!;
    public Applicant Applicant { get; set; } = null!;
}