using Serilog;

namespace ReelSchedulerPro.Worker;

public class PostingWorker : BackgroundService
{
    private readonly ILogger<PostingWorker> _logger;

    public PostingWorker(ILogger<PostingWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PostingWorker starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // TODO: Implement posting logic
                _logger.LogInformation("Posting worker executing at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PostingWorker");
            }
        }

        _logger.LogInformation("PostingWorker stopping...");
    }
}
