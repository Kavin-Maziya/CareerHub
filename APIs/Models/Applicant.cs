namespace APIs.Models;

public class Applicant
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

//currently commented out for simplicity applicants will use their emails instead
//public string PhoneNumber { get; set; } = string.Empty; // Phone number stored as string

// Navigation property to Applications - one applicant can have many applications
    public ICollection<Application> Applications { get; set; } = [];
}