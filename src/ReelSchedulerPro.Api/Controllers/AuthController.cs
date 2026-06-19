using Microsoft.AspNetCore.Mvc;
using ReelSchedulerPro.Application.Services;
using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Domain.Entities;
using ReelSchedulerPro.Shared.DTOs;
using FluentValidation;
using ReelSchedulerPro.Application.Validators;

namespace ReelSchedulerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokenService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtTokenService tokenService,
        ApplicationDbContext dbContext,
        IValidator<LoginRequest> loginValidator,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
        _loginValidator = loginValidator;
        _logger = logger;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT tokens</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Login validation failed for {Email}", request.Email);
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for {Email}: Invalid credentials", request.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed for {Email}: User is inactive", request.Email);
                return Unauthorized(new { message = "User account is inactive" });
            }

            var token = _tokenService.GenerateToken(user.Id, user.Email, user.Role, user.OrganizationId);
            _logger.LogInformation("User {Email} logged in successfully", user.Email);

            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New JWT tokens</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                _logger.LogWarning("Token refresh failed: No refresh token provided");
                return BadRequest(new { message = "Refresh token is required" });
            }

            // TODO: Validate refresh token against database
            // For now, generate new token
            var token = new AuthTokenDTO
            {
                AccessToken = "new_access_token",
                RefreshToken = request.RefreshToken,
                ExpiresIn = 3600
            };

            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh error");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
