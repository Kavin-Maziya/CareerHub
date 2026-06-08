using APIs.Models;
using Microsoft.EntityFrameworkCore;

namespace APIs.Data;

public class CareerHubDbContext(
    DbContextOptions<CareerHubDbContext> options)
    : DbContext(options)
{
    public DbSet<JobListing> JobListings => Set<JobListing>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<Applicant> Applicants => Set<Applicant>();

    public DbSet<Application> Applications => Set<Application>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobListing>(entity =>
        {
            entity.ToTable("job_listings");

            entity.HasKey(j => j.Id);

            entity.Property(j => j.Id)
                .ValueGeneratedNever();

            entity.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(j => j.Description)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(j => j.Location)
                .IsRequired()
                .HasMaxLength(200);

            // Unique constraint no duplicate title and company
            entity.HasIndex(j => new { j.Title, j.CompanyId })
                .IsUnique();

            entity.HasOne(j => j.Company)
                .WithMany(c => c.JobListings)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // SalaryMin must be greater than zero when provided
            entity.ToTable(t => t.HasCheckConstraint(
                "ck_job_listings_salary_min_positive",
                "\"SalaryMin\" IS NULL OR \"SalaryMin\" > 0"));

            // SalaryMax must be greater than SalaryMin when provided
            entity.ToTable(t => t.HasCheckConstraint(
                "ck_job_listings_salary_max_gt_min",
                "\"SalaryMin\" IS NULL OR \"SalaryMax\" IS NULL OR \"SalaryMax\" > \"SalaryMin\""));

            // ClosingDate must be after PostedAt
            entity.ToTable(t => t.HasCheckConstraint(
                "ck_job_listings_closing_after_posted",
                "\"ClosingDate\" > \"PostedAt\""));

            // Active listing query: WHERE IsActive = true AND ClosingDate > now()
            
            entity.HasIndex(j => new { j.IsActive, j.ClosingDate })
                .HasDatabaseName("ix_job_listings_is_active_closing_date");

            // Company-scoped listing query: WHERE CompanyId = X AND IsActive = true
            entity.HasIndex(j => new { j.CompanyId, j.IsActive })
                .HasDatabaseName("ix_job_listings_company_id_is_active");

            // Full-text search GIN index on computed tsvector column
            entity.HasIndex("SearchVector")
                .HasDatabaseName("ix_job_listings_search_vector")
                .HasMethod("GIN");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("companies");

            entity.HasKey(c => c.CompanyId);

            entity.Property(c => c.CompanyId)
                .ValueGeneratedNever();

            entity.Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(c => c.Industry)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.ToTable("applicants");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .ValueGeneratedNever();

            entity.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(a => a.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("applications");

            // Composite primary key to prevent duplicate applications
            entity.HasKey(a => new { a.JobListingId, a.ApplicantId });

            entity.Property(a => a.SubmittedAt)
                .IsRequired();

            entity.Property(a => a.Status)
                .IsRequired();

            // Application and JobListing relationship
            entity.HasOne(a => a.JobListing)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobListingId);

            // Application and Applicant relationship
            entity.HasOne(a => a.Applicant)
                .WithMany(a => a.Applications)
                .HasForeignKey(a => a.ApplicantId);

            // SubmittedAt cannot be in the future
            entity.ToTable(t => t.HasCheckConstraint(
                "ck_applications_submitted_at_not_future",
                "\"SubmittedAt\" <= now()"));

            entity.HasIndex(a => a.JobListingId)
                .HasDatabaseName("ix_applications_job_listing_id");

            entity.HasIndex(a => a.ApplicantId)
                .HasDatabaseName("ix_applications_applicant_id");
        });
    }
}
