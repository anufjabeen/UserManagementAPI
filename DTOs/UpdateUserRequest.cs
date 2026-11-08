using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTOs;

/// <summary>Body of PUT /users/{id}. Kept separate from the create DTO so the two can evolve independently.</summary>
public class UpdateUserRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not a valid email address.")]
    [StringLength(100, ErrorMessage = "Email must be 100 characters or fewer.")]
    public string? Email { get; set; }

    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Department must be between 2 and 50 characters.")]
    public string? Department { get; set; }
}
