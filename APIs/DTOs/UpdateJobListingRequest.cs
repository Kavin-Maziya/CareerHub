namespace APIs.DTOs;

public record UpdateJobListingRequest
{
    public string? Title;
    public string? Description;
    public string? Location;
    public string? EmploymentType;
    public decimal? SalaryMin;
    public decimal? SalaryMax;
    public DateTime? ExpiresAt;
}