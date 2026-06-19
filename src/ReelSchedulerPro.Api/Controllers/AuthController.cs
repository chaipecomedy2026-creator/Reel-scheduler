using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ReelSchedulerPro.Application.Services;
using ReelSchedulerPro.Application.Validators;
using ReelSchedulerPro.Shared.DTOs;
using Serilog;

namespace ReelSchedulerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly ILogger _logger = Log.ForContext<AuthController>();

    public AuthController(
        IAuthenticationService authService,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator)
    {
        _authService = authService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            var response = await _authService.RegisterAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.OrganizationName,
                cancellationToken);

            _logger.Information("User registered: {Email}", request.Email);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Registration error");
            return StatusCode(500, new { message = ex.Message });
        }
    }

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
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            var token = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
            _logger.Information("User logged in: {Email}", request.Email);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Warning("Login failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Login error");
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
