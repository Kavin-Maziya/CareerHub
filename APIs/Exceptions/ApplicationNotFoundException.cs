namespace APIs.Exceptions;

public class ApplicationNotFoundException : Exception
{
    public ApplicationNotFoundException(Guid jobListingId, Guid applicantId)
        : base($"Application for job listing '{jobListingId}' and applicant '{applicantId}' was not found.")
    {
    }
}