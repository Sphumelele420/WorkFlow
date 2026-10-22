
using System.ComponentModel.DataAnnotations;

namespace WorkFlow.Api.DTOs;

public class EmployeeRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string JobTitle { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Department { get; set; }

    public DateTime? DateHired { get; set; }
}
