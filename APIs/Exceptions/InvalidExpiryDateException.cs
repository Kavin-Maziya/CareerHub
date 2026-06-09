namespace APIs.Exceptions;
public class InvalidExpiryDateException : Exception
{
    public InvalidExpiryDateException() 
        : base("The expiry date must be a future date.") { }
}