using System.Security.Claims;
using ReelSchedulerPro.Shared.DTOs;

namespace ReelSchedulerPro.Application.Services;

public interface IJwtTokenService
{
    AuthTokenDTO GenerateToken(Guid userId, string email, string role, Guid organizationId);
    ClaimsPrincipal? ValidateToken(string token);
}
