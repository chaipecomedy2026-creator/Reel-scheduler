namespace ReelSchedulerPro.Domain.Entities;

public class CaptionHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Prompt { get; set; }
    public required string GeneratedCaption { get; set; }
    public string? GeneratedHashtags { get; set; }
    public string? Model { get; set; }
    public int TokensUsed { get; set; }
    public decimal CostEstimate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
}
