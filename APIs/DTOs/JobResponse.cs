namespace APIs.DTOs;

public record JobListResponse(
    Guid Id,
    string Title,
    string CompanyName,
    string Location,
    string SalaryDisplay,
    int ApplicationCount
);

public record JobDetailResponse(
    Guid Id,
    string Title,
    Guid CompanyId,
    string CompanyName,
    string Location,
    string Description,
    string SalaryDisplay,
    DateTime PostedAt,
    bool IsActive,
    List<ApplicationSummary> Applications
);

public record ApplicationSummary(
    string ApplicantName,
    DateTime SubmittedAt,
    string Status
);