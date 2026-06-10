using System.ComponentModel.DataAnnotations;
using APIs.Models;
using APIs.Controllers;
using System.Data;

namespace APIs.DTOs;

public record CreateJobRequest(
    string Title, string CompanyName, string Industry, string Location, 
    string Description, JobType Type, DateTime ClosingDate, 
    decimal? SalaryMin, decimal? SalaryMax
) : JobRequestBase(Title, CompanyName, Industry, Location, Description, Type, ClosingDate, SalaryMin, SalaryMax);

public abstract record JobRequestBase(
    [Required(ErrorMessage = "Job Title is required")]
    [StringLength(120, MinimumLength = 5)]
    string Title,

    [Required(ErrorMessage = "Company name is required")]
    [StringLength(150, MinimumLength = 2)]
    string CompanyName,

    [StringLength(100)]
    string Industry,

    [Required]
    string Location,

    [Required]
    [MinLength(20)]
    string Description,

    [Required]
    JobType Type,

    [Required]
    DateTime ClosingDate,

    [Range(0.01, double.MaxValue)]
    decimal? SalaryMin,

    [Range(0.01, double.MaxValue)]
    decimal? SalaryMax
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {

        if (ClosingDate <= DateTime.UtcNow)
        {
            yield return new ValidationResult(
                "Closing date must be an upcoming date",
                new[] { nameof(ClosingDate) });
        }

        if (SalaryMin.HasValue &&
            SalaryMax.HasValue &&
            SalaryMax <= SalaryMin)
        {
            yield return new ValidationResult(
                "SalaryMax must be greater than SalaryMin",
                new[] { nameof(SalaryMax) });
        }
    }
}