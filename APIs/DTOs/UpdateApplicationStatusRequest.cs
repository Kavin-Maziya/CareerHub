using System.ComponentModel.DataAnnotations;
using APIs.Models;

namespace APIs.DTOs;

public record UpdateApplicationStatusRequest
{
    [Required]
    public ApplicationStatus Status;
    
}