namespace APIs.Exceptions;

public class InvalidSalaryRangeException : Exception
{
    public InvalidSalaryRangeException() 
        : base("SalaryMax must be greater than or equal to SalaryMin.") { }
}