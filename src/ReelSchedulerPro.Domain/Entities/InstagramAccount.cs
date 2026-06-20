namespace ReelSchedulerPro.Domain.Entities;

public class InstagramAccount
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DisconnectedAt { get; set; }
    public DateTime? LastHealthCheckAt { get; set; }
    public string? HealthCheckMessage { get; set; }

    // Navigation
    public User? User { get; set; }
    public Organization? Organization { get; set; }
    public ICollection<ScheduledReel> ScheduledReels { get; set; } = new List<ScheduledReel>();
}

public enum AccountStatus
{
    Active,
    Inactive,
    Disconnected,
    TokenExpired
}
