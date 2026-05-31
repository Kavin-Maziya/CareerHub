namespace APIs.Exceptions;

public class JobNotFoundException : Exception
{
    public JobNotFoundException(Guid id): base($"Job Listing with ID {id} was not found")
    {
        
    }
}