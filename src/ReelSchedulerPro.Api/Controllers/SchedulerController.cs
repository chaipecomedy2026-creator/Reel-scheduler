using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ReelSchedulerPro.Application.Commands;
using ReelSchedulerPro.Shared.DTOs;
using System.Security.Claims;
using FluentValidation;
using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Application.Validators;

namespace ReelSchedulerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchedulerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateScheduledReelDTO> _validator;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SchedulerController> _logger;

    public SchedulerController(
        IMediator mediator,
        IValidator<CreateScheduledReelDTO> validator,
        ApplicationDbContext dbContext,
        ILogger<SchedulerController> logger)
    {
        _mediator = mediator;
        _validator = validator;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Schedule a new reel for posting
    /// </summary>
    /// <param name="request">Reel scheduling details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Scheduled reel ID</returns>
    [HttpPost("schedule-reel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ScheduleReel([FromBody] CreateScheduledReelDTO request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Schedule reel validation failed");
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? Guid.Empty.ToString());
            
            if (organizationId == Guid.Empty)
            {
                _logger.LogWarning("Schedule reel failed: No organization ID in claims");
                return Unauthorized(new { message = "Organization not found in token" });
            }

            var command = new ScheduleReelCommand
            {
                OrganizationId = organizationId,
                ReelData = request
            };

            var reelId = await _mediator.Send(command, cancellationToken);
            _logger.LogInformation("Reel {ReelId} scheduled successfully", reelId);

            return Ok(new { reelId, message = "Reel scheduled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule reel");
            return StatusCode(500, new { message = "Failed to schedule reel" });
        }
    }

    /// <summary>
    /// Get all scheduled reels for the organization
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of scheduled reels</returns>
    [HttpGet("scheduled-reels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetScheduledReels(CancellationToken cancellationToken)
    {
        try
        {
            var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? Guid.Empty.ToString());
            
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { message = "Organization not found in token" });
            }

            var reels = _dbContext.ScheduledReels
                .Where(r => r.OrganizationId == organizationId)
                .OrderByDescending(r => r.ScheduledFor)
                .ToList();

            _logger.LogInformation("Retrieved {Count} scheduled reels", reels.Count);
            return Ok(new { data = reels, count = reels.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve scheduled reels");
            return StatusCode(500, new { message = "Failed to retrieve scheduled reels" });
        }
    }
}
