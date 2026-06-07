namespace APIs.Exceptions;

public class ListingClosedException : Exception
{
    public ListingClosedException(Guid id)
        : base($"Job listing '{id}' is closed and no longer accepting applications.")
    {
    }
}