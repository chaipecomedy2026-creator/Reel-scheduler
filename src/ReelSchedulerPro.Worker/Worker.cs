using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Application.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Hangfire;

namespace ReelSchedulerPro.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger _logger = Log.ForContext<Worker>();
    private readonly IServiceProvider _serviceProvider;
    private PeriodicTimer? _timer;

    public Worker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Information("Reel Scheduler Worker started");
        
        using (var scope = _serviceProvider.CreateScope())
        {
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            
            recurringJobManager.AddOrUpdate(
                "process-scheduled-reels",
                () => ProcessScheduledReels(CancellationToken.None),
                "*/30 * * * * *"); // Every 30 seconds

            recurringJobManager.AddOrUpdate(
                "check-account-health",
                () => CheckAccountHealth(CancellationToken.None),
                "0 */1 * * * *"); // Every hour
        }
        
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        try
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessScheduledReels(stoppingToken);
                await CheckAccountHealth(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Information("Worker cancellation requested");
        }
        finally
        {
            _timer?.Dispose();
        }
    }

    private async Task ProcessScheduledReels(CancellationToken cancellationToken)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var instagramService = scope.ServiceProvider.GetRequiredService<IInstagramService>();
                var encryptionService = scope.ServiceProvider.GetRequiredService<IEncryptionService>();
                
                var now = DateTime.UtcNow;

                var reelsToPost = await dbContext.ScheduledReels
                    .Where(r => r.Status == "Pending" && r.ScheduledFor <= now)
                    .ToListAsync(cancellationToken);

                if (reelsToPost.Count > 0)
                {
                    _logger.Information("Processing {Count} reels", reelsToPost.Count);
                }

                foreach (var reel in reelsToPost)
                {
                    try
                    {
                        var account = await dbContext.InstagramAccounts
                            .FirstOrDefaultAsync(a => a.Id == reel.InstagramAccountId, cancellationToken);

                        if (account == null)
                        {
                            _logger.Warning("Instagram account not found for reel {ReelId}", reel.Id);
                            reel.Status = "Failed";
                            reel.FailureReason = "Account not found";
                            continue;
                        }

                        var decryptedToken = encryptionService.Decrypt(account.AccessToken);
                        account.AccessToken = decryptedToken;

                        var success = await instagramService.PostReelAsync(account, reel, cancellationToken);

                        if (success)
                        {
                            reel.Status = "Published";
                            _logger.Information("Reel {ReelId} posted successfully", reel.Id);
                        }
                        else
                        {
                            reel.RetryCount++;
                            if (reel.RetryCount >= 3)
                            {
                                reel.Status = "Failed";
                                reel.FailureReason = "Max retries exceeded";
                                _logger.Error("Reel {ReelId} failed after {Retries} retries", reel.Id, reel.RetryCount);
                            }
                            else
                            {
                                reel.Status = "Pending";
                                reel.ScheduledFor = DateTime.UtcNow.AddMinutes(5);
                                _logger.Warning("Reel {ReelId} retry {Attempt}", reel.Id, reel.RetryCount);
                            }
                        }

                        reel.UpdatedAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error processing reel {ReelId}", reel.Id);
                        reel.Status = "Failed";
                        reel.FailureReason = ex.Message;
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in ProcessScheduledReels");
        }
    }

    private async Task CheckAccountHealth(CancellationToken cancellationToken)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var instagramService = scope.ServiceProvider.GetRequiredService<IInstagramService>();

                var accountsToCheck = await dbContext.InstagramAccounts
                    .Where(a => a.LastHealthCheckAt < DateTime.UtcNow.AddHours(-1))
                    .ToListAsync(cancellationToken);

                foreach (var account in accountsToCheck)
                {
                    try
                    {
                        var isHealthy = await instagramService.CheckAccountHealthAsync(account, cancellationToken);
                        account.IsHealthy = isHealthy;
                        account.LastHealthCheckAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error checking health for account {AccountId}", account.Id);
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in CheckAccountHealth");
        }
    }
}
