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

    // Part 5: Generated, stored tsvector column for full-text search.
    // Computed by PostgreSQL from Title + Description using the english config.
    // EF Core treats this as a regular property; the migration will add the
    // GENERATED ALWAYS AS expression via raw SQL in the migration file.
    public NpgsqlTsVector? SearchVector { get; set; }

    // Navigation property to Applications
    public ICollection<Application> Applications { get; set; } = [];
}
