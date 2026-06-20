using ReelSchedulerPro.Domain.Entities;
using ReelSchedulerPro.Shared.DTOs;

namespace ReelSchedulerPro.Application.Services;

public interface ISchedulerService
{
    Task<ScheduledReel?> CreateScheduledReelAsync(CreateScheduledReelRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<ScheduledReel?> UpdateScheduledReelAsync(Guid reelId, UpdateScheduledReelRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteScheduledReelAsync(Guid reelId, Guid userId, CancellationToken cancellationToken = default);
    Task<ScheduledReel?> GetScheduledReelAsync(Guid reelId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduledReel>> GetScheduledReelsAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> RetryFailedPostAsync(Guid reelId, CancellationToken cancellationToken = default);
}
