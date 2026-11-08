namespace UserManagementAPI.DTOs;

/// <summary>What the API returns to clients. Decouples the public contract from the internal User entity.</summary>
public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Department { get; set; } = string.Empty;
}
