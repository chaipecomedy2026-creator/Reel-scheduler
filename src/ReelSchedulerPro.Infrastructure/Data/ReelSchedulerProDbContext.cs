using Microsoft.EntityFrameworkCore;
using ReelSchedulerPro.Domain.Entities;

namespace ReelSchedulerPro.Infrastructure.Data;

public class ReelSchedulerProDbContext : DbContext
{
    public ReelSchedulerProDbContext(DbContextOptions<ReelSchedulerProDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Organization> Organizations { get; set; } = null!;
    public DbSet<InstagramAccount> InstagramAccounts { get; set; } = null!;
    public DbSet<ScheduledReel> ScheduledReels { get; set; } = null!;
    public DbSet<PostingLog> PostingLogs { get; set; } = null!;
    public DbSet<CaptionHistory> CaptionHistories { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Organization Configuration
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // InstagramAccount Configuration
        modelBuilder.Entity<InstagramAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(256);
            entity.Property(e => e.AccessToken).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.InstagramAccounts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Organization)
                .WithMany(o => o.InstagramAccounts)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ScheduledReel Configuration
        modelBuilder.Entity<ScheduledReel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VideoUrl).IsRequired();
            entity.Property(e => e.Caption).IsRequired();
            entity.HasOne(e => e.InstagramAccount)
                .WithMany(a => a.ScheduledReels)
                .HasForeignKey(e => e.InstagramAccountId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ScheduledTime);
            entity.HasIndex(e => e.Status);
        });

        // PostingLog Configuration
        modelBuilder.Entity<PostingLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ScheduledReel)
                .WithMany(r => r.PostingLogs)
                .HasForeignKey(e => e.ScheduledReelId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.CreatedAt);
        });

        // CaptionHistory Configuration
        modelBuilder.Entity<CaptionHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GeneratedCaption).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshToken Configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Token).IsUnique();
        });
    }
}
