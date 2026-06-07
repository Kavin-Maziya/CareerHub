using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs;

public record UpdateApplicationRequest(
    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("UnderReview|Shortlisted|Offered|Rejected",
        ErrorMessage = "Status must be one of: UnderReview, Shortlisted, Offered, Rejected")]
    string Status
);