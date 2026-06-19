using ReelSchedulerPro.Shared.DTOs;

namespace ReelSchedulerPro.Application.Services;

public interface IAuthenticationService
{
    Task<AuthTokenDTO> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<AuthTokenDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<RegisterResponse> RegisterAsync(string email, string password, string firstName, string lastName, string organizationName, CancellationToken cancellationToken);
}
