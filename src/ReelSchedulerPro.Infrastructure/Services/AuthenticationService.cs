using Microsoft.AspNetCore.Identity;
using ReelSchedulerPro.Application.Services;
using ReelSchedulerPro.Domain.Entities;
using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Shared.DTOs.Authentication;
using ReelSchedulerPro.Shared.Exceptions;
using Serilog;

namespace ReelSchedulerPro.Infrastructure.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly ReelSchedulerProDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        ReelSchedulerProDbContext dbContext,
        ITokenService tokenService,
        IPasswordHasher<ApplicationUser> passwordHasher,
        ILogger<AuthenticationService> logger)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            throw new ValidationException("Email and password are required");

        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Login failed: User not found or inactive - {Email}", request.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login failed: Invalid password - {Email}", request.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _logger.LogInformation("Login successful for email: {Email}", request.Email);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToString(),
                    ProfilePictureUrl = user.ProfilePictureUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ValidationException("Registration request is required");

        try
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            // Check if email already exists
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", request.Email);
                throw new ValidationException("Email already registered", "EMAIL_EXISTS", 409);
            }

            // Create organization
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = request.OrganizationName,
                Slug = request.OrganizationName.ToLower().Replace(" ", "-"),
                SubscriptionTier = Domain.Entities.SubscriptionTier.Free,
                IsActive = true
            };

            // Create user
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.FullName,
                OrganizationId = organization.Id,
                Role = Domain.Entities.UserRole.Admin, // First user is admin
                IsActive = true,
                EmailVerified = false
            };

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // Save to database
            await _dbContext.Organizations.AddAsync(organization, cancellationToken);
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _logger.LogInformation("Registration successful for email: {Email}", request.Email);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToString(),
                    ProfilePictureUrl = user.ProfilePictureUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException("Refresh token is required");

        try
        {
            _logger.LogInformation("Token refresh attempt");

            // TODO: Implement refresh token storage and validation
            // For now, we'll generate a new token for a valid request
            // In production, validate against stored refresh tokens in database

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            _logger.LogInformation("Token refreshed successfully");

            return new LoginResponse
            {
                AccessToken = "new-access-token", // Generate new token
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh error");
            throw;
        }
    }

    public async Task<bool> VerifyEmailAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            throw new ValidationException("Email and verification code are required");

        try
        {
            _logger.LogInformation("Email verification attempt for: {Email}", email);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Email verification failed: User not found - {Email}", email);
                throw new NotFoundException("User not found");
            }

            // TODO: Implement email verification code validation
            // For now, mark as verified
            user.EmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Email verified successfully for: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email verification error for: {Email}", email);
            throw;
        }
    }
}
