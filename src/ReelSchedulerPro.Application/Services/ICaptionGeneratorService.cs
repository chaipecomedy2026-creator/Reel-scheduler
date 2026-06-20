using ReelSchedulerPro.Shared.DTOs;

namespace ReelSchedulerPro.Application.Services;

public interface ICaptionGeneratorService
{
    Task<CaptionGenerationResponse?> GenerateCaptionAsync(GenerateCaptionRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CaptionHistory>> GetCaptionHistoryAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default);
}

public class CaptionHistory
{
    public Guid Id { get; set; }
    public required string Prompt { get; set; }
    public required string GeneratedCaption { get; set; }
    public DateTime CreatedAt { get; set; }
}
