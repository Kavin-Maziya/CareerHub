namespace APIs.Models;
//Join table between Applicant and JobListing - represents a job application submitted by an applicant for a specific job listing
public class Application // Represents a job application submitted by an applicant for a specific job listing
{
    //public Guid Id {get; set;}
    public Guid JobListingId { get; set; }

    public Guid ApplicantId { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public ApplicationStatus Status { get; set; }

    public JobListing JobListing { get; set; } = null!;

    public Applicant Applicant { get; set; } = null!;
}