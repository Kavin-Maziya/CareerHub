using System.Text.Json.Serialization;

namespace APIs.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApplicationStatus
{
    Submitted,
    UnderReview,
    Shortlisted,
    Offered,
    Rejected
}