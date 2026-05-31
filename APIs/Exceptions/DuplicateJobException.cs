namespace APIs.Exceptions;

public class DuplicateBookingException : Exception
{
    public DuplicateBookingException(string title, string company): 
    base($"A job listing for {title} at {company} already exists")
    {
        
    }
}