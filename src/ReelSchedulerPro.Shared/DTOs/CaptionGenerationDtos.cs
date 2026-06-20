namespace ReelSchedulerPro.Shared.DTOs;

public class GenerateCaptionRequest
{
    public required string Topic { get; set; }
    public required string Style { get; set; }
    public bool IncludeHashtags { get; set; } = true;
    public int MaxLength { get; set; } = 2200;
}

public class CaptionGenerationResponse
{
    public required string Caption { get; set; }
    public string? Hashtags { get; set; }
    public int TokensUsed { get; set; }
    public decimal CostEstimate { get; set; }
}
