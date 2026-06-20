namespace ReelSchedulerPro.Shared.DTOs;

public class CreateScheduledReelRequest
{
    public Guid InstagramAccountId { get; set; }
    public required string VideoUrl { get; set; }
    public required string Caption { get; set; }
    public string? Hashtags { get; set; }
    public string? AltText { get; set; }
    public DateTime ScheduledTime { get; set; }
    public string TimeZone { get; set; } = "UTC";
}

public class UpdateScheduledReelRequest
{
    public required string Caption { get; set; }
    public string? Hashtags { get; set; }
    public string? AltText { get; set; }
    public DateTime ScheduledTime { get; set; }
}

public class ScheduledReelDto
{
    public Guid Id { get; set; }
    public required string Caption { get; set; }
    public string? Hashtags { get; set; }
    public DateTime ScheduledTime { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}
