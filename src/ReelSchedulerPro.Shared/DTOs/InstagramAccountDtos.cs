namespace ReelSchedulerPro.Shared.DTOs;

public class ConnectInstagramAccountRequest
{
    public required string AccessToken { get; set; }
}

public class InstagramAccountDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Status { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? LastHealthCheckAt { get; set; }
    public string? HealthCheckMessage { get; set; }
}
