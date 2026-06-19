namespace ReelSchedulerPro.Shared.Constants;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Creator = "Creator";
    public const string Viewer = "Viewer";
    
    public static readonly string[] AllRoles = { Admin, Creator, Viewer };
}

public static class JobStatuses
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Queued = "Queued";
    public const string Scheduled = "Scheduled";
}

public static class ReelStatuses
{
    public const string Draft = "Draft";
    public const string Pending = "Pending";
    public const string Scheduled = "Scheduled";
    public const string Publishing = "Publishing";
    public const string Published = "Published";
    public const string Failed = "Failed";
    public const string Archived = "Archived";
}

public static class ErrorMessages
{
    public const string InvalidCredentials = "Invalid email or password";
    public const string UserNotFound = "User not found";
    public const string EmailAlreadyExists = "Email already exists";
    public const string InvalidToken = "Invalid or expired token";
    public const string UnauthorizedAccess = "Unauthorized access";
    public const string InternalServerError = "An internal server error occurred";
}
