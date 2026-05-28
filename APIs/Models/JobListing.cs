namespace APIs.Models;

public class JobListing
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public DateTime PostedAt{get; set;}
    public bool IsActive{get; set;}

}