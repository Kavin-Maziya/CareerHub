namespace APIs.DTOs;

public record ApplicationResponse(
    Guid Id,
    Guid JobListingId,
    Guid ApplicantId,
    string JobTitle,
    string ApplicantName,
    DateTime SubmittedAt,
    string Status);
