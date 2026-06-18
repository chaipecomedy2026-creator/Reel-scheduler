using FluentValidation;
using ReelSchedulerPro.Api.Filters;

namespace ReelSchedulerPro.Api;

/// <summary>
/// Extension methods for API service configuration
/// </summary>
public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Add FluentValidation
        services.AddValidatorsFromAssembly(typeof(ReelSchedulerPro.Shared.Constants.ApiConstants).Assembly);

        // Add global filters
        services.AddScoped<GlobalExceptionFilter>();
        services.AddScoped<ModelValidationFilter>();

        // Configure MVC options
        services.AddControllers(options =>
        {
            options.Filters.AddService<GlobalExceptionFilter>();
            options.Filters.AddService<ModelValidationFilter>();
        });

        return services;
    }
}
