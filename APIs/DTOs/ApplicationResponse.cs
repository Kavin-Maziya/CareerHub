// namespace APIs.DTOs;

// public record ApplicationResponse(
//     Guid Id,
//     Guid JobListingId,
//     Guid ApplicantId,
//     string JobTitle,
//     string ApplicantName,
//     DateTime SubmittedAt,
//     string Status);

using System;
using APIs.Models;
namespace APIs.DTOs;

public record ApplicationResponse(
    Guid Id,
    Guid JobListingId,
    Guid ApplicantId,
    string JobTitle,
    string ApplicantName,
    string Email,
    string? Phone,
    int YearsOfExperience,
    string CoverLetter,
    string? LinkedInUrl,
    bool AvailableImmediately,
    int NoticePeriodWeeks,
    DateTime SubmittedAt,
    ApplicationStatus Status
);