using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReelSchedulerPro.Application.Services;
using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Domain.Entities;
using System.Security.Claims;

namespace ReelSchedulerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InstagramController : ControllerBase
{
    private readonly IInstagramService _instagramService;
    private readonly IEncryptionService _encryptionService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<InstagramController> _logger;

    public InstagramController(
        IInstagramService instagramService,
        IEncryptionService encryptionService,
        ApplicationDbContext dbContext,
        ILogger<InstagramController> logger)
    {
        _instagramService = instagramService;
        _encryptionService = encryptionService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Connect Instagram account
    /// </summary>
    /// <param name="request">Connection details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Connected account</returns>
    [HttpPost("connect-account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConnectAccount([FromBody] ConnectInstagramRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.Handle))
            {
                return BadRequest(new { message = "AccessToken and Handle are required" });
            }

            var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? Guid.Empty.ToString());
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { message = "Organization not found" });
            }

            var isValid = await _instagramService.ValidateAccessTokenAsync(request.AccessToken, cancellationToken);
            if (!isValid)
            {
                _logger.LogWarning("Invalid Instagram access token");
                return BadRequest(new { message = "Invalid access token" });
            }

            var encryptedToken = _encryptionService.Encrypt(request.AccessToken);
            var account = new InstagramAccount
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                InstagramHandle = request.Handle,
                InstagramUserId = request.UserId,
                AccessToken = encryptedToken,
                TokenExpiresAt = DateTime.UtcNow.AddMonths(2),
                IsHealthy = true
            };

            _dbContext.InstagramAccounts.Add(account);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Instagram account {Handle} connected", request.Handle);
            return Ok(new { accountId = account.Id, handle = account.InstagramHandle });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect Instagram account");
            return StatusCode(500, new { message = "Failed to connect account" });
        }
    }

    /// <summary>
    /// Get connected Instagram accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of accounts</returns>
    [HttpGet("accounts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetAccounts(CancellationToken cancellationToken)
    {
        try
        {
            var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? Guid.Empty.ToString());
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { message = "Organization not found" });
            }

            var accounts = _dbContext.InstagramAccounts
                .Where(a => a.OrganizationId == organizationId)
                .Select(a => new
                {
                    a.Id,
                    a.InstagramHandle,
                    a.IsHealthy,
                    a.LastHealthCheckAt,
                    a.CreatedAt
                })
                .ToList();

            return Ok(new { data = accounts, count = accounts.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve Instagram accounts");
            return StatusCode(500, new { message = "Failed to retrieve accounts" });
        }
    }

    /// <summary>
    /// Check account health
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health status</returns>
    [HttpGet("check-health/{accountId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckHealth(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? Guid.Empty.ToString());
            var account = _dbContext.InstagramAccounts
                .FirstOrDefault(a => a.Id == accountId && a.OrganizationId == organizationId);

            if (account == null)
            {
                return NotFound(new { message = "Account not found" });
            }

            var isHealthy = await _instagramService.CheckAccountHealthAsync(account, cancellationToken);
            account.IsHealthy = isHealthy;
            account.LastHealthCheckAt = DateTime.UtcNow;

            _dbContext.InstagramAccounts.Update(account);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new { isHealthy, lastCheck = account.LastHealthCheckAt });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check account health");
            return StatusCode(500, new { message = "Failed to check health" });
        }
    }
}

public class ConnectInstagramRequest
{
    public string Handle { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}
