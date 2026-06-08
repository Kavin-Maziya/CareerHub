using APIs.Models;

namespace APIs.Exceptions;

public class InvalidStatusTransitionException : Exception
{
    public InvalidStatusTransitionException(ApplicationStatus from, ApplicationStatus to)
        : base($"Invalid status transition from '{from}' to '{to}'.")
    {
    }

    public InvalidStatusTransitionException(string status)
        : base($"'{status}' is not a valid application status.")
    {
    }
}