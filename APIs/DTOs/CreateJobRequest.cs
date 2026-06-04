using System.ComponentModel.DataAnnotations;
using APIs.Models;
using APIs.Controllers;
using System.Data;


namespace APIs.DTOs;

//Request Dto - What the client sends to the backend to create a booking

public record CreateJobRequest 
(

    [Required(ErrorMessage="Job Title is required when posting a job")]
    [StringLength(120, MinimumLength = 5, ErrorMessage ="Job Title should be between 5 and 120 characters")]
    string Title,

    [Required(ErrorMessage = "Company is required")]
    Guid CompanyId,

    [Required(ErrorMessage ="Location is required")]
    string Location,

    [Required(ErrorMessage = "Job description is required")]
    [MinLength(20, ErrorMessage ="Job description should be a minimum 20 characters")]
    string Description,

    [Required(ErrorMessage="Job Type is required, must be one of: FullTime, PartTime, Contract, Internship")]
    JobType Type,

    [Range(0.01, double.MaxValue, ErrorMessage ="Salary must be greater than zero")]
    decimal? SalaryMin,

    [Range(0.01, double.MaxValue, ErrorMessage ="Salary must be greater than zero")]
    decimal? SalaryMax
    
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (SalaryMin.HasValue &&
            SalaryMax.HasValue &&
            SalaryMax <= SalaryMin)
        {
            yield return new ValidationResult(
                "SalaryMax must be greater than SalaryMin",
                new[] { nameof(SalaryMax) });
        }
    } }