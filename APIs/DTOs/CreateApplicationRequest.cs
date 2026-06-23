// using System.ComponentModel.DataAnnotations;

// namespace APIs.DTOs;

// public record CreateApplicationRequest
// (
//     [Required(ErrorMessage = "Job Listing Id is required")]
//     Guid JobListingId,

//     [Required(ErrorMessage = "First name is required")]
//     [StringLength(80, MinimumLength = 2,
//         ErrorMessage = "First name must be between 2 and 80 characters")]
//     string FirstName,

//     [Required(ErrorMessage = "Last name is required")]
//     [StringLength(80, MinimumLength = 2,
//         ErrorMessage = "Last name must be between 2 and 80 characters")]
//     string LastName,

//     [Required(ErrorMessage = "Email is required")]
//     [EmailAddress(ErrorMessage = "Invalid email format")]
//     [StringLength(150, ErrorMessage = "Email must be under 150 characters")]
//     string Email,

//     [Phone(ErrorMessage = "Invalid phone number format")]
//     [StringLength(20, ErrorMessage = "Phone number must be under 20 characters")]
//     string? PhoneNumber
// ) : IValidatableObject
// {
//     public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//     {
//         // basic defensive validation for names (ensures they aren't just whitespace)
//         if (string.IsNullOrWhiteSpace(FirstName))
//         {
//             yield return new ValidationResult(
//                 "First name cannot be empty or whitespace",
//                 new[] { nameof(FirstName) });
//         }

//         if (string.IsNullOrWhiteSpace(LastName))
//         {
//             yield return new ValidationResult(
//                 "Last name cannot be empty or whitespace",
//                 new[] { nameof(LastName) });
//         }

//         // email normalization safety check
//         if (!string.IsNullOrWhiteSpace(Email) &&
//             Email.Contains(" "))
//         {
//             yield return new ValidationResult(
//                 "Email cannot contain spaces",
//                 new[] { nameof(Email) });
//         }
//     }
// }

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs;

public record CreateApplicationRequest
(
    [Required(ErrorMessage = "Job Listing Id is required")]
    Guid JobListingId,

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be at least 2 characters")]
    string FullName,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(150, ErrorMessage = "Email must be under 150 characters")]
    string Email,

    [RegularExpression(@"^\+?[\d\s\-()\d]{8,15}$", ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number must be under 20 characters")]
    string? Phone,

    [Required(ErrorMessage = "Years of experience is required")]
    [Range(0, 50, ErrorMessage = "Maximum 50 years")]
    int YearsOfExperience,

    [Required(ErrorMessage = "Cover letter is required")]
    [StringLength(2000, MinimumLength = 50, ErrorMessage = "Cover letter must be at least 50 characters — tell us why you're a strong fit")]
    string CoverLetter,

    [Url(ErrorMessage = "Please enter a valid URL")]
    [StringLength(200, ErrorMessage = "URL must be under 200 characters")]
    string? LinkedInUrl,

    [Required(ErrorMessage = "Availability status flag is required")]
    bool AvailableImmediately,

    [Range(0, 52, ErrorMessage = "Notice period must be a valid number of weeks")]
    int NoticePeriodWeeks
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(FullName) && string.IsNullOrWhiteSpace(FullName.Trim()))
        {
            yield return new ValidationResult(
                "Full name cannot be empty or whitespace.",
                new[] { nameof(FullName) });
        }

        if (!AvailableImmediately && NoticePeriodWeeks < 1)
        {
            yield return new ValidationResult(
                "Notice period must be at least 1 week if you are not available immediately.",
                new[] { nameof(NoticePeriodWeeks) });
        }
    }
}