namespace APIs.DTOs;

public record JobListingFilterQuery
{
    public string? Location { get; init; }
    public string? EmploymentType { get; init; }
    public decimal? SalaryMin { get; init; }
    public decimal? SalaryMax { get; init; }
    public Guid? CompanyId { get; init; }
    public string Sort { get; init; } = "postedAt";
    public string Dir { get; init; } = "desc";
}