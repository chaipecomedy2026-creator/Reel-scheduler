using System.Security.Claims;
using ReelSchedulerPro.Application.Services;

namespace ReelSchedulerPro.Api.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IJwtTokenService tokenService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var principal = tokenService.ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;
                    _logger.LogInformation("JWT token validated for user {Email}", 
                        principal.FindFirst(ClaimTypes.Email)?.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "JWT validation failed");
            }
        }

        await _next(context);
    }
}
