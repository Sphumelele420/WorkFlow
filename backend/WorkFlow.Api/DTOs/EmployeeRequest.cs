
using System.ComponentModel.DataAnnotations;

namespace WorkFlow.Api.DTOs;

public class EmployeeRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "First name must be between 2 and 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Last name must be between 2 and 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    [StringLength(255,
        ErrorMessage = "Email cannot exceed 255 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Job title must be between 2 and 100 characters.")]
    public string JobTitle { get; set; } = string.Empty;

    [StringLength(100,
        ErrorMessage = "Department cannot exceed 100 characters.")]
    public string? Department { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateHired { get; set; }
}
