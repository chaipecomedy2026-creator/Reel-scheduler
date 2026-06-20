using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Worker;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add Hangfire
        services.AddHangfire(cfg => cfg.UsePostgreSqlStorage(context.Configuration.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();

        // Add DbContext
        services.AddDbContext<ReelSchedulerProDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));

        // Add Hosted Services
        services.AddHostedService<PostingWorker>();
        services.AddHostedService<HealthCheckWorker>();
    })
    .UseSerilog((context, services, logger) => logger
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day))
    .Build();

await host.RunAsync();
