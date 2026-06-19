using ReelSchedulerPro.Domain.Entities;

namespace ReelSchedulerPro.Infrastructure.Data;

public static class SeedData
{
    public static void SeedDatabase(ApplicationDbContext context)
    {
        if (context.Organizations.Any())
        {
            return; // Database already seeded
        }

        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Demo Organization",
            SubscriptionPlan = "Premium",
            SubscriptionExpiresAt = DateTime.UtcNow.AddYears(1),
            IsActive = true
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@demo.com",
            FirstName = "Admin",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin",
            OrganizationId = org.Id,
            IsActive = true
        };

        var instagramAccount = new InstagramAccount
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            InstagramHandle = "demo_account",
            InstagramUserId = "123456789",
            AccessToken = "demo_token_encrypted",
            TokenExpiresAt = DateTime.UtcNow.AddMonths(2),
            IsHealthy = true
        };

        var scheduledReel = new ScheduledReel
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            InstagramAccountId = instagramAccount.Id,
            VideoUrl = "https://example.com/video.mp4",
            Caption = "Check out this amazing reel! 🎬✨",
            Hashtags = "#Instagram #Reels #Marketing",
            ScheduledFor = DateTime.UtcNow.AddDays(1),
            Status = "Pending",
            Timezone = "UTC"
        };

        var captionHistory = new CaptionHistory
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            Prompt = "Create an engaging caption for a product launch video",
            GeneratedCaption = "Check out this amazing reel! 🎬✨",
            Hashtags = "#Instagram #Reels #Marketing",
            AiModel = "gpt-4"
        };

        context.Organizations.Add(org);
        context.Users.Add(user);
        context.InstagramAccounts.Add(instagramAccount);
        context.ScheduledReels.Add(scheduledReel);
        context.CaptionHistories.Add(captionHistory);

        context.SaveChanges();
    }
}
