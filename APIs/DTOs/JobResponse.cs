using APIs.Models;

namespace APIs.DTOs; 

public record JobResponse
(
    Guid Id,
    string Title,
    Guid CompanyId,
    string Location,
    string Description,
    JobType Type,
    decimal? SalaryMin,
    decimal? SalaryMax,
    DateTime PostedAt,
    bool IsActive,
    string SalaryDisplay
);