namespace ReelSchedulerPro.Domain.Entities;

/// <summary>
/// Application user entity for authentication and authorization
/// </summary>
public class ApplicationUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public Guid OrganizationId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailVerified { get; set; } = false;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public Organization Organization { get; set; } = null!;
}

public enum UserRole
{
    Admin = 0,
    Manager = 1,
    User = 2
}
