namespace APIs.Exceptions;

public class DuplicateApplicationException : Exception
{
    public DuplicateApplicationException(Guid jobListingId, Guid applicantId)
        : base($"Applicant '{applicantId}' has already applied for job listing '{jobListingId}'.")
    {
    }
}