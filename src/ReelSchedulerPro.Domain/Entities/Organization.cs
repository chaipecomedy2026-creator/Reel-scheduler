namespace ReelSchedulerPro.Domain.Entities;

public class Organization
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Logo { get; set; }
    public required string Slug { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;
    public int MaxAccounts { get; set; } = 1;
    public int MaxScheduledReels { get; set; } = 10;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<InstagramAccount> InstagramAccounts { get; set; } = new List<InstagramAccount>();
}

public enum SubscriptionTier
{
    Free,
    Starter,
    Professional,
    Enterprise
}
