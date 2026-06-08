namespace APIs.DTOs;

// Part 8: Result record for the raw SQL stats query.
// Fields map directly to the column aliases in GetApplicationStatsAsync.
public record JobListingStatsResponse(
    Guid JobListingId,
    string Title,
    int TotalApplications,
    int Submitted,
    int UnderReview,
    int Shortlisted,
    int Offered,
    int Rejected,
    long Rank
);
