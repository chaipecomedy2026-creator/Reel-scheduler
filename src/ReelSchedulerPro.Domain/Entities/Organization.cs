namespace ReelSchedulerPro.Domain.Entities;

/// <summary>
/// Organization entity representing a tenant in the multi-tenant SaaS system
/// </summary>
public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<InstagramAccount> InstagramAccounts { get; set; } = new List<InstagramAccount>();
    public ICollection<ScheduledReel> ScheduledReels { get; set; } = new List<ScheduledReel>();
    public ICollection<CaptionHistory> CaptionHistories { get; set; } = new List<CaptionHistory>();
}

public enum SubscriptionTier
{
    Free = 0,
    Basic = 1,
    Professional = 2,
    Enterprise = 3
}
