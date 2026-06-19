using FluentValidation;
using ReelSchedulerPro.Shared.DTOs;

namespace ReelSchedulerPro.Application.Validators;

public class ScheduleReelValidator : AbstractValidator<CreateScheduledReelDTO>
{
    public ScheduleReelValidator()
    {
        RuleFor(x => x.InstagramAccountId)
            .NotEmpty().WithMessage("Instagram Account ID is required");

        RuleFor(x => x.VideoUrl)
            .NotEmpty().WithMessage("Video URL is required")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Video URL must be a valid URL");

        RuleFor(x => x.Caption)
            .NotEmpty().WithMessage("Caption is required")
            .MaximumLength(2200).WithMessage("Caption cannot exceed 2200 characters");

        RuleFor(x => x.ScheduledFor)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Scheduled time must be in the future");

        RuleFor(x => x.Timezone)
            .NotEmpty().WithMessage("Timezone is required")
            .Must(IsValidTimezone).WithMessage("Invalid timezone");
    }

    private static bool IsValidTimezone(string timezone)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
