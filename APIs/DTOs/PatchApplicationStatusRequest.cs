using System.ComponentModel.DataAnnotations;
using APIs.Models;

namespace APIs.DTOs;

public record PatchApplicationStatusRequest
{
    [Required]
    public ApplicationStatus Status { get; init; }
}