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
public class CaptionController : ControllerBase
{
    private readonly IAiCaptionService _captionService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CaptionController> _logger;

    public CaptionController(
        IAiCaptionService captionService,
        ApplicationDbContext dbContext,
        ILogger<CaptionController> logger)
    {
        _captionService = captionService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Generate AI-powered caption
    /// </summary>
    /// <param name="prompt">Caption prompt</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated caption</returns>
    [HttpPost("generate-caption")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerateCaption([FromBody] GenerateCaptionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { message = "Prompt is required" });
            }

            var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? Guid.Empty.ToString());
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { message = "Organization not found" });
            }

            _logger.LogInformation("Generating caption for organization {OrgId}", organizationId);
            var caption = await _captionService.GenerateCaptionAsync(request.Prompt, cancellationToken);

            var history = new CaptionHistory
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Prompt = request.Prompt,
                GeneratedCaption = caption,
                AiModel = "gpt-4"
            };

            _dbContext.CaptionHistories.Add(history);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new { caption, historyId = history.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate caption");
            return StatusCode(500, new { message = "Failed to generate caption" });
        }
    }

    /// <summary>
    /// Generate hashtags for a caption
    /// </summary>
    /// <param name="request">Caption text</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated hashtags</returns>
    [HttpPost("generate-hashtags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerateHashtags([FromBody] GenerateHashtagsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Caption))
            {
                return BadRequest(new { message = "Caption is required" });
            }

            var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? Guid.Empty.ToString());
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { message = "Organization not found" });
            }

            _logger.LogInformation("Generating hashtags");
            var hashtags = await _captionService.GenerateHashtagsAsync(request.Caption, cancellationToken);

            return Ok(new { hashtags });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate hashtags");
            return StatusCode(500, new { message = "Failed to generate hashtags" });
        }
    }

    /// <summary>
    /// Get caption generation history
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Caption history</returns>
    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        try
        {
            var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? Guid.Empty.ToString());
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { message = "Organization not found" });
            }

            var history = _dbContext.CaptionHistories
                .Where(h => h.OrganizationId == organizationId)
                .OrderByDescending(h => h.CreatedAt)
                .Take(50)
                .ToList();

            return Ok(new { data = history, count = history.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve caption history");
            return StatusCode(500, new { message = "Failed to retrieve history" });
        }
    }
}

public class GenerateCaptionRequest
{
    public string Prompt { get; set; } = string.Empty;
}

public class GenerateHashtagsRequest
{
    public string Caption { get; set; } = string.Empty;
}
