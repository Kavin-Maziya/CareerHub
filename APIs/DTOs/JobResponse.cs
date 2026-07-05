using System.Text.Json.Serialization;
using APIs.Models;

namespace APIs.DTOs;

public record JobListResponse(
    Guid Id,
    string Title,
    string CompanyName,
    string Location,
    string Description,
    DateTime PostedAt,
    string SalaryDisplay,
    DateTime ClosingDate,
    int ApplicationCount,
    bool IsActive,                                        
   [property: JsonPropertyName("employmentType")]
EmploymentType EmploymentType
);
// {
//     public JobListResponse(Guid Id, string Title, string CompanyName, string Location, string SalaryDisplay, int ApplicationCount, DateTime ClosingDate)
//         : this(Id, Title, CompanyName, Location, DateTime.UtcNow, SalaryDisplay, ClosingDate, ApplicationCount, true)
//     {
//     }
// }

public record JobDetailResponse(
    Guid Id,
    string Title,
    Guid CompanyId,
    string CompanyName,
    string Location,
    string Description,
    string SalaryDisplay,
    DateTime PostedAt,
    DateTime ClosingDate,
    bool IsActive,
    [property: JsonPropertyName("employmentType")]
    EmploymentType EmploymentType,
    List<ApplicationSummary> Applications
);

public record ApplicationSummary(
    string ApplicantName,
    DateTime SubmittedAt,
    string Status
);
