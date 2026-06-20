namespace ReelSchedulerPro.Domain.Entities;

public class PostingLog
{
    public Guid Id { get; set; }
    public Guid ScheduledReelId { get; set; }
    public PostingLogStatus Status { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ScheduledReel? ScheduledReel { get; set; }
}

public enum PostingLogStatus
{
    Info,
    Warning,
    Error,
    Success
}
