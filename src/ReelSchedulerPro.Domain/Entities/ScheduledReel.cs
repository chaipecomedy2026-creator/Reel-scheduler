namespace ReelSchedulerPro.Domain.Entities;

public class ScheduledReel
{
    public Guid Id { get; set; }
    public Guid InstagramAccountId { get; set; }
    public required string VideoUrl { get; set; }
    public required string Caption { get; set; }
    public string? Hashtags { get; set; }
    public string? AltText { get; set; }
    public DateTime ScheduledTime { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public ReelStatus Status { get; set; } = ReelStatus.Scheduled;
    public int RetryAttempts { get; set; } = 0;
    public int MaxRetries { get; set; } = 3;
    public string? PostId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public InstagramAccount? InstagramAccount { get; set; }
    public ICollection<PostingLog> PostingLogs { get; set; } = new List<PostingLog>();
}

public enum ReelStatus
{
    Scheduled,
    Processing,
    Posted,
    Failed,
    Cancelled
}
