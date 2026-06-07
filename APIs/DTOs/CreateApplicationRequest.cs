using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs;

public record CreateApplicationRequest
(
    [Required(ErrorMessage = "Job Listing Id is required")]
    Guid JobListingId,

    [Required(ErrorMessage = "First name is required")]
    [StringLength(80, MinimumLength = 2,
        ErrorMessage = "First name must be between 2 and 80 characters")]
    string FirstName,

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(80, MinimumLength = 2,
        ErrorMessage = "Last name must be between 2 and 80 characters")]
    string LastName,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(150, ErrorMessage = "Email must be under 150 characters")]
    string Email,

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number must be under 20 characters")]
    string? PhoneNumber
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // basic defensive validation for names
        if (FirstName.Trim().Length == 0)
        {
            yield return new ValidationResult(
                "First name cannot be empty or whitespace",
                new[] { nameof(FirstName) });
        }

        if (LastName.Trim().Length == 0)
        {
            yield return new ValidationResult(
                "Last name cannot be empty or whitespace",
                new[] { nameof(LastName) });
        }

        // email normalization safety check
        if (!string.IsNullOrWhiteSpace(Email) &&
            Email.Contains(" "))
        {
            yield return new ValidationResult(
                "Email cannot contain spaces",
                new[] { nameof(Email) });
        }
    }
}