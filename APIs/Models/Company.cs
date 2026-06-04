
namespace APIs.Models;
public class Company
{
    public Guid CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Industry { get; set; } = string.Empty;
//Navigation property to JobListings - one company can have many job listings
    public ICollection<JobListing> JobListings { get; set; } = [];
}