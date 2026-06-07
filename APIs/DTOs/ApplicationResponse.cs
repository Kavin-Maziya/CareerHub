namespace APIs.DTOs;

public record ApplicationResponse(
    Guid JobListingId,
    Guid ApplicantId,
    string ApplicantName,
    DateTime SubmittedAt,
    string Status
);

