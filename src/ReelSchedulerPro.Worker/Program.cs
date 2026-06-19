using ReelSchedulerPro.Infrastructure.Data;
using ReelSchedulerPro.Application.Services;
using ReelSchedulerPro.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

Host.CreateDefaultBuilder(args)
    .UseSerilog((context, configuration) =>
        configuration
            .WriteTo.Console()
            .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day)
            .MinimumLevel.Information()
            .Enrich.FromLogContext())
    .ConfigureServices((context, services) =>
    {
        // Add DbContext
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Database=ReelSchedulerPro;Username=postgres;Password=postgres";
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Add Services
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IInstagramService, InstagramService>();
        services.AddHttpClient<IInstagramService, InstagramService>();

        services.AddHostedService<ReelSchedulerPro.Worker.Worker>();
    })
    .Build()
    .RunAsync();
