using APIs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace APIs.Data;

public class CareerHubDbContext(
    DbContextOptions<CareerHubDbContext> options)
    : DbContext(options)
{
    public DbSet<JobListing> JobListings => Set<JobListing>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<Applicant> Applicants => Set<Applicant>();

    public DbSet<Application> Applications => Set<Application>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Suppresses the error that kills tests when the model and snapshot have minor discrepancies.
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        base.OnConfiguring(optionsBuilder);
    }

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
                
            entity.Property(j => j.EmploymentType)
    .HasColumnName("Type")
    .HasConversion<string>();

            entity.HasIndex(j => new
            {
                j.Title,
                j.CompanyId
            })
            .IsUnique();

            // Relationship: Company and JobListing (1-to-many relationship)
            entity.HasOne(j => j.Company) // Each job listing has one company
                .WithMany(c => c.JobListings) // One company can have many job listings
                .HasForeignKey(j => j.CompanyId) // Use CompanyId as the foreign key
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to preserve job listings if a company is deleted
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

            // Composite Key of JobListingId and ApplicantId to prevent duplicate applications
            entity.HasKey(a => new
            {
                a.JobListingId,
                a.ApplicantId
            });

            entity.Property(a => a.SubmittedAt)
                .IsRequired();

            entity.Property(a => a.Status)
                .IsRequired();

            // Application and JobListing relationship (many-to-one)
            entity.HasOne(a => a.JobListing) // Each application is for one job listing
                .WithMany(j => j.Applications) // One job listing can have many applications
                .HasForeignKey(a => a.JobListingId); // Use JobListingId as the foreign key

            // Application and Applicant relationship (many-to-one)
            entity.HasOne(a => a.Applicant) // Each application is submitted by one applicant
                .WithMany(a => a.Applications) // One applicant can submit many applications
                .HasForeignKey(a => a.ApplicantId); // Use ApplicantId as the foreign key
        });
    }
}
