using NpgsqlTypes;

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

    // Generated Tsvector column for full-text search.
    public NpgsqlTsVector? SearchVector { get; set; }

    // Navigation property to Applications
    public ICollection<Application> Applications { get; set; } = [];
}
