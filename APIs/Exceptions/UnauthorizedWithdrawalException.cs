namespace APIs.Exceptions;

public class UnauthorizedWithdrawalException : Exception
{
    public UnauthorizedWithdrawalException(Guid applicantId)
        : base($"Applicant '{applicantId}' is not authorized to withdraw this application.")
    {
    }
}