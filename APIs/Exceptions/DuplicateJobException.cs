namespace APIs.Exceptions;

public class DuplicateJobException : Exception
{
    public DuplicateJobException(string title, string company): 
    base($"A job listing for {title} at {company} already exists")
    {
        
    }
}