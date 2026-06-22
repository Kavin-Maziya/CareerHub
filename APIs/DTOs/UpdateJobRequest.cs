using System.ComponentModel.DataAnnotations;
using APIs.Models;
using APIs.Controllers;
using System.Data;

namespace APIs.DTOs;

public record UpdateJobRequest(
    string Title, string CompanyName, string Industry, string Location, 
    string Description, EmploymentType Type, DateTime ClosingDate, 
    decimal? SalaryMin, decimal? SalaryMax
) : JobRequestBase(Title, CompanyName, Industry, Location, Description, Type, ClosingDate, SalaryMin, SalaryMax);