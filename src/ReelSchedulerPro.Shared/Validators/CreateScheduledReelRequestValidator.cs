using FluentValidation;
using ReelSchedulerPro.Shared.DTOs.Reels;

namespace ReelSchedulerPro.Shared.Validators;

/// <summary>
/// Validator for CreateScheduledReelRequest
/// </summary>
public class CreateScheduledReelRequestValidator : AbstractValidator<CreateScheduledReelRequest>
{
    public CreateScheduledReelRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.VideoUrl)
            .NotEmpty().WithMessage("Video URL is required")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _)).WithMessage("Video URL must be a valid URL");

        RuleFor(x => x.Caption)
            .NotEmpty().WithMessage("Caption is required")
            .MinimumLength(10).WithMessage("Caption must be at least 10 characters")
            .MaximumLength(2200).WithMessage("Caption must not exceed 2200 characters");

        RuleFor(x => x.Hashtags)
            .MaximumLength(500).WithMessage("Hashtags must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Hashtags));

        RuleFor(x => x.ScheduledPostTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled post time must be in the future");

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone is required")
            .Must(IsValidTimeZone).WithMessage("Invalid time zone ID");

        RuleFor(x => x.InstagramAccountId)
            .NotEmpty().WithMessage("Instagram account ID is required");
    }

    private static bool IsValidTimeZone(string timeZoneId)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
