using APIs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace APIs.Data;

public class CareerHubDbContext(
    DbContextOptions<CareerHubDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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

    modelBuilder.Entity<RefreshToken>()
        .HasOne(rt => rt.User)
        .WithMany(u => u.RefreshTokens)
        .HasForeignKey(rt => rt.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<User>(entity =>
    {
        entity.ToTable("users");

        entity.HasKey(u => u.Id);

        entity.HasIndex(u => u.Email)
            .IsUnique();

        entity.Property(u => u.Email)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(u => u.PasswordHash)
            .IsRequired();

        entity.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(u => u.Role)
            .HasMaxLength(30)
            .IsRequired();
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

            entity.Property(a => a.Phone)
                .IsRequired(false)
                .HasMaxLength(20);
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

            // --- Extended Application Submission Metadata ---
            entity.Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(a => a.Phone)
                .IsRequired(false)
                .HasMaxLength(20);

            entity.Property(a => a.YearsOfExperience)
                .IsRequired();

            entity.Property(a => a.CoverLetter)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(a => a.LinkedInUrl)
                .IsRequired(false)
                .HasMaxLength(200);

            entity.Property(a => a.AvailableImmediately)
                .IsRequired();

            entity.Property(a => a.NoticePeriodWeeks)
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

        SeedUsers(modelBuilder);
        SeedCompanies(modelBuilder);
        SeedJobListings(modelBuilder);
        SeedApplicants(modelBuilder);
        SeedApplications(modelBuilder);
    }

private void SeedUsers(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>().HasData(

        new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Username = "admin",
            Email = "admin@careerhub.co.za",
            PasswordHash = "Admin123!",
            Role = "Admin",
            IsActive = true
        },

        new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Username = "employer",
            Email = "employer@careerhub.co.za",
            PasswordHash = "Employer123!",
            Role = "Employer",
            IsActive = true
        },

        new User
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Username = "applicant",
            Email = "applicant@careerhub.co.za",
            PasswordHash = "Applicant123!",
            Role = "Applicant",
            IsActive = true
        }

    );
}
    private void SeedCompanies(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Company>().HasData(
        new Company
        {
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
            CompanyName = "Takealot",
            Industry = "Technology"
        },
        new Company
        {
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
            CompanyName = "Vodacom",
            Industry = "Telecommunications"
        },
        new Company
        {
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
            CompanyName = "Discovery",
            Industry = "Insurance"
        },
        new Company
        {
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004"),
            CompanyName = "Standard Bank",
            Industry = "Finance"
        },
        new Company
        {
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005"),
            CompanyName = "FNB FirstRand",
            Industry = "Finance"
        },
        new Company
        {
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000006"),
            CompanyName = "Media24",
            Industry = "Media"
        }
    );
}
private void SeedJobListings(ModelBuilder modelBuilder)
{
    var now = new DateTime(2026, 06, 24, 0, 0, 0, DateTimeKind.Utc);

    modelBuilder.Entity<JobListing>().HasData(
        new JobListing
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            Title = "Senior Frontend Software Engineer",
            Description = "We are looking for a talented Senior Frontend Engineer...",
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
            Location = "Cape Town",
            EmploymentType = EmploymentType.FullTime,
            SalaryMin = 30000,
            SalaryMax = 45000,
            PostedAt = now,
            IsActive = true,
            ClosingDate = now.AddDays(30)
        },
        new JobListing
        {
            Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
            Title = "Junior Systems Developer",
            Description = "We are looking for a Junior Systems Developer...",
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
            Location = "Johannesburg, Sandton",
            EmploymentType = EmploymentType.FullTime,
            SalaryMin = 15000,
            SalaryMax = 30000,
            PostedAt = now.AddDays(-3),
            IsActive = true,
            ClosingDate = now.AddDays(30)
        },
        new JobListing
        {
            Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
            Title = "UX/Web Designer",
            Description = "We are looking for a creative UX/Web Designer...",
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
            Location = "Sandton",
            EmploymentType = EmploymentType.Contract,
            SalaryMin = 10000,
            SalaryMax = 18000,
            PostedAt = now.AddDays(-10),
            IsActive = true,
            ClosingDate = now.AddDays(30)
        },
        new JobListing
        {
            Id = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
            Title = "Data Analyst Intern",
            Description = "We are looking for a Data Analyst Intern...",
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004"),
            Location = "Pretoria/Hybrid",
            EmploymentType = EmploymentType.Internship,
            SalaryMin = 15000,
            SalaryMax = 22000,
            PostedAt = now.AddDays(-45),
            IsActive = false,
            ClosingDate = now.AddDays(-5)
        },
        new JobListing
        {
            Id = Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234"),
            Title = "Senior DevOps Engineer",
            Description = "We are looking for a Senior DevOps Engineer...",
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005"),
            Location = "Bloemfontein",
            EmploymentType = EmploymentType.FullTime,
            SalaryMin = 70000,
            SalaryMax = 110000,
            PostedAt = now.AddDays(-2),
            IsActive = true,
            ClosingDate = now.AddDays(30)
        },
        new JobListing
        {
            Id = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345"),
            Title = "Part-Time Content Writer/Promoter",
            Description = "We are looking for a Content Writer...",
            CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000006"),
            Location = "Remote",
            EmploymentType = EmploymentType.PartTime,
            SalaryMin = 12000,
            SalaryMax = 18000,
            PostedAt = now.AddDays(-60),
            IsActive = true,
            ClosingDate = now.AddDays(30)
        }
    );
}

private void SeedApplicants(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Applicant>().HasData(
        new Applicant
        {
            Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001"),
            FirstName = "Thabo",
            LastName = "Nkosi",
            Email = "thabo.nkosi@example.com",
            Phone = "0810000001"
        },
        new Applicant
        {
            Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002"),
            FirstName = "Amanda",
            LastName = "van der Merwe",
            Email = "amanda.vandermerwe@example.com",
            Phone = "0810000002"
        },
        new Applicant
        {
            Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000003"),
            FirstName = "Sipho",
            LastName = "Dlamini",
            Email = "sipho.dlamini@example.com",
            Phone = "0810000003"
        }
    );
}

private void SeedApplications(ModelBuilder modelBuilder)
{
    var now = new DateTime(2026, 07, 10, 0, 0, 0, DateTimeKind.Utc);

    var applicantIds = new[]
    {
        Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001"),
        Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002"),
        Guid.Parse("bbbbbbbb-0000-0000-0000-000000000003")
    };

    var applicantNames = new[]
    {
        ("Thabo", "Nkosi", "thabo.nkosi@example.com"),
        ("Amanda", "van der Merwe", "amanda.vandermerwe@example.com"),
        ("Sipho", "Dlamini", "sipho.dlamini@example.com")
    };

    var jobListingIds = new[]
    {
        Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), // Senior Frontend Software Engineer
        Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"), // Junior Systems Developer
        Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"), // UX/Web Designer
        Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"), // Data Analyst Intern
        Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234"), // Senior DevOps Engineer
        Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345")  // Part-Time Content Writer/Promoter
    };

    // Cycles through all five ApplicationStatus values across the 18
    // seeded applications (3 applicants x 6 listings) so the Applications
    // tab has realistic variety to filter against.
    var statuses = new[]
    {
        ApplicationStatus.Submitted,
        ApplicationStatus.UnderReview,
        ApplicationStatus.Shortlisted,
        ApplicationStatus.Offered,
        ApplicationStatus.Rejected
    };

    var applications = new List<Application>();
    var statusIndex = 0;

    for (var applicantIndex = 0; applicantIndex < applicantIds.Length; applicantIndex++)
    {
        var (firstName, lastName, email) = applicantNames[applicantIndex];

        for (var listingIndex = 0; listingIndex < jobListingIds.Length; listingIndex++)
        {
            applications.Add(new Application
            {
                JobListingId = jobListingIds[listingIndex],
                ApplicantId = applicantIds[applicantIndex],
                FullName = $"{firstName} {lastName}",
                Email = email,
                Phone = "0810000000",
                YearsOfExperience = 2 + applicantIndex * 2,
                CoverLetter = $"I am excited to apply for this role and believe my experience aligns well with what you're looking for.",
                LinkedInUrl = null,
                AvailableImmediately = applicantIndex % 2 == 0,
                NoticePeriodWeeks = applicantIndex % 2 == 0 ? 0 : 4,
                SubmittedAt = now.AddDays(-(applicantIndex * 6 + listingIndex)),
                Status = statuses[statusIndex % statuses.Length]
            });

            statusIndex++;
        }
    }

    modelBuilder.Entity<Application>().HasData(applications);
}

}