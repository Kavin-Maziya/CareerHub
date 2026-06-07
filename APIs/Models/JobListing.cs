namespace APIs.Models;

public class JobListing
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    // Foreign Key
    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;


    public string Location { get; set; } = string.Empty;

    public JobType Type { get; set; }

    public DateTime ClosingDate { get; set; }

    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }

    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;


// Navigation property to Applications - one job listing can have many applications
    public ICollection<Application> Applications { get; set; } = [];


    // public JobListing( 
    //     Guid id, string title, string description, 
    //     string company, string location, JobType type, 
    //     decimal salaryMin, decimal salaryMax, DateTime postedAt, bool isActive)
    //     {
    //         Id = id; 
    //         Title = title;
    //         Description = description;
    //         Company = company;
    //         Location = location;
    //         Type = type;
    //         SalaryMin = salaryMin;
    //         SalaryMax = salaryMax;
    //         PostedAt = postedAt;
    //         IsActive = isActive;

    //     }

}