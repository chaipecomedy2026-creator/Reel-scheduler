using MediatR;
using ReelSchedulerPro.Application.Commands;
using ReelSchedulerPro.Domain.Entities;
using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Shared.DTOs;
using Serilog;

namespace ReelSchedulerPro.Application.Handlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger = Log.ForContext<RegisterUserCommandHandler>();

    public RegisterUserCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already exists");
        }

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.OrganizationName,
            SubscriptionPlan = "Free",
            IsActive = true
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Creator",
            OrganizationId = organization.Id,
            IsActive = true
        };

        _context.Organizations.Add(organization);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.Information("User {Email} registered successfully", user.Email);

        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}
