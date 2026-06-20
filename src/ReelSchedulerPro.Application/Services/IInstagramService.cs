using ReelSchedulerPro.Domain.Entities;
using ReelSchedulerPro.Shared.DTOs;

namespace ReelSchedulerPro.Application.Services;

public interface IInstagramService
{
    Task<InstagramAccount?> ConnectAccountAsync(ConnectInstagramAccountRequest request, Guid userId, Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> DisconnectAccountAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InstagramAccount>> GetUserAccountsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> PostReelAsync(Guid accountId, Guid reelId, CancellationToken cancellationToken = default);
    Task<bool> CheckAccountHealthAsync(Guid accountId, CancellationToken cancellationToken = default);
}
