namespace APIs.Exceptions;

public class UnauthorizedCompanyException : Exception
{
    public UnauthorizedCompanyException(Guid id)
        : base($"You are not authorized to update job listing '{id}' as it belongs to a different company.")
    {
    }
}