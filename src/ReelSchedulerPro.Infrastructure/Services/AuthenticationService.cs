using ReelSchedulerPro.Application.Commands;
using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Shared.DTOs;
using MediatR;
using Serilog;

namespace ReelSchedulerPro.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly ILogger _logger = Log.ForContext<AuthenticationService>();

    public AuthenticationService(
        IJwtTokenService jwtTokenService,
        ApplicationDbContext dbContext,
        IMediator mediator)
    {
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task<AuthTokenDTO> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.Warning("Login failed for {Email}: Invalid credentials", email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            _logger.Warning("Login failed for {Email}: User is inactive", email);
            throw new UnauthorizedAccessException("User account is inactive");
        }

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role, user.OrganizationId);
        _logger.Information("User {Email} logged in successfully", email);
        return token;
    }

    public async Task<AuthTokenDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var token = new AuthTokenDTO
        {
            AccessToken = refreshToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600
        };

        return await Task.FromResult(token);
    }

    public async Task<RegisterResponse> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string organizationName,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand
        {
            Email = email,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            OrganizationName = organizationName
        };

        return await _mediator.Send(command, cancellationToken);
    }
}
