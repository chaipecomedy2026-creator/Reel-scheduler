using ReelSchedulerPro.Application.Services;
using ReelSchedulerPro.Domain.Entities;
using Serilog;

namespace ReelSchedulerPro.Infrastructure.Services;

public class InstagramService : IInstagramService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _apiVersion;
    private readonly ILogger _logger = Log.ForContext<InstagramService>();

    public InstagramService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiUrl = configuration["Instagram:ApiUrl"] ?? "https://graph.instagram.com";
        _apiVersion = configuration["Instagram:ApiVersion"] ?? "v18.0";
    }

    public async Task<bool> PostReelAsync(InstagramAccount account, ScheduledReel reel, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Posting reel {ReelId} to Instagram account {AccountId}", reel.Id, account.Id);

            var request = new HttpRequestMessage(HttpMethod.Post, 
                $"{_apiUrl}/{_apiVersion}/{account.InstagramUserId}/media")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "media_type", "REELS" },
                    { "video_url", reel.VideoUrl },
                    { "caption", reel.Caption },
                    { "access_token", account.AccessToken }
                })
            };

            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.Information("Reel {ReelId} posted successfully", reel.Id);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.Error("Failed to post reel {ReelId}: {Error}", reel.Id, errorContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error posting reel {ReelId}", reel.Id);
            return false;
        }
    }

    public async Task<bool> CheckAccountHealthAsync(InstagramAccount account, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Checking health for Instagram account {AccountId}", account.Id);

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_apiUrl}/{_apiVersion}/{account.InstagramUserId}?access_token={account.AccessToken}");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.Information("Instagram account {AccountId} is healthy", account.Id);
                return true;
            }

            _logger.Warning("Instagram account {AccountId} health check failed with status {Status}", 
                account.Id, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error checking Instagram account {AccountId} health", account.Id);
            return false;
        }
    }

    public async Task<bool> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Validating Instagram access token");

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_apiUrl}/debug_token?input_token={accessToken}&access_token={accessToken}");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.Information("Instagram access token is valid");
                return true;
            }

            _logger.Warning("Instagram access token validation failed");
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error validating Instagram access token");
            return false;
        }
    }
}
