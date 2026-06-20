namespace ReelSchedulerPro.Shared.Constants;

public static class AppConstants
{
    public const string DefaultTimeZone = "UTC";
    public const int JwtExpirationMinutes = 15;
    public const int RefreshTokenExpirationDays = 7;
    public const int MaxRetryAttempts = 3;
    public const int AccountHealthCheckIntervalHours = 1;
    public const int MaxScheduledReelsPerOrganization = 1000;
}

public static class ClaimTypes
{
    public const string UserId = "userId";
    public const string OrganizationId = "organizationId";
    public const string Role = "role";
    public const string Email = "email";
}
