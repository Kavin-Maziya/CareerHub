namespace APIs.DTOs;

public record ApplicationResponse(
    Guid JobListingId,
    Guid ApplicantId,
    string JobTitle,
    string ApplicantName,
    DateTime SubmittedAt,
    string Status
,
    object Id);

