using ReelSchedulerPro.Application.Services;
using Serilog;

namespace ReelSchedulerPro.Infrastructure.Services;

public class AiCaptionService : IAiCaptionService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly ILogger _logger = Log.ForContext<AiCaptionService>();

    public AiCaptionService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
        _model = configuration["OpenAI:Model"] ?? "gpt-4";
    }

    public async Task<string> GenerateCaptionAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.Warning("OpenAI API key not configured, returning mock caption");
                return GenerateMockCaption(prompt);
            }

            _logger.Information("Generating caption with OpenAI for prompt: {Prompt}", prompt);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        model = _model,
                        messages = new[] { new { role = "user", content = prompt } },
                        max_tokens = 150
                    }),
                    System.Text.Encoding.UTF8,
                    "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.Information("Caption generated successfully");
                return ParseCaptionFromResponse(content);
            }

            _logger.Error("Failed to generate caption: {Status}", response.StatusCode);
            return GenerateMockCaption(prompt);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating caption");
            return GenerateMockCaption(prompt);
        }
    }

    public async Task<string> GenerateHashtagsAsync(string caption, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Generating hashtags for caption");

            var prompt = $"Generate 10 relevant hashtags for this caption: {caption}. Return only hashtags, comma separated.";
            var hashtags = await GenerateCaptionAsync(prompt, cancellationToken);
            
            return hashtags;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating hashtags");
            return "#instagram #reels #content";
        }
    }

    public async Task<string> GenerateHookAsync(string topic, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Generating hook for topic: {Topic}", topic);

            var prompt = $"Generate a catchy, engaging hook for a social media post about {topic}. Make it attention-grabbing and under 30 words.";
            var hook = await GenerateCaptionAsync(prompt, cancellationToken);
            
            return hook;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating hook");
            return "Check this out! You won't believe what happens next...";
        }
    }

    private static string GenerateMockCaption(string prompt)
    {
        return $"Amazing content about {prompt}! 🎬✨ Don't miss out! #Instagram #Reels #ContentCreator";
    }

    private static string ParseCaptionFromResponse(string jsonResponse)
    {
        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(jsonResponse);
            var message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
            return message ?? "Unable to generate caption";
        }
        catch
        {
            return "Unable to parse caption response";
        }
    }
}
