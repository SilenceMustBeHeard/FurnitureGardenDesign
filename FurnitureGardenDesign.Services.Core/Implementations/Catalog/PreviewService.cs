using FurnitureGardenDesign.Services.Core.Interfaces.Catalog;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Services.Core.Implementations.Catalog
{
    public class PreviewService : IPreviewService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PreviewService> _logger;

        public PreviewService(
            IHttpClientFactory httpClientFactory,
            ILogger<PreviewService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<(bool Success, string? ImageUrl, string? Message)> FetchWebpageImageAsync(string url)
        {
            // Validate URL
            if (string.IsNullOrWhiteSpace(url) 
                || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                _logger.LogWarning("Invalid URL provided: {Url}", url);
                return (false, null, "Invalid URL provided.");
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                var html = await httpClient.GetStringAsync(url);

                // Try Open Graph image
                var ogImageMatch = Regex.Match(html,
                    @"<meta\s+property=[""]og:image[""]\s+content=[""]([^""]*)[""]",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (ogImageMatch.Success)
                {
                    var imageUrl = ogImageMatch.Groups[1].Value;

                    // Make URL absolute if relative
                    if (!imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        var baseUri = new Uri(url);
                        imageUrl = new Uri(baseUri, imageUrl).ToString();
                    }

                    _logger.LogInformation("Found Open Graph image for {Url}", url);
                    return (true, imageUrl, null);
                }

                // Fallback to first image
                var imgMatch = Regex.Match(html,
                    @"<img[^>]*src=[""]([^""]*)[""][^>]*>",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (imgMatch.Success)
                {
                    var imageUrl = imgMatch.Groups[1].Value;
                    _logger.LogInformation("Found fallback image for {Url}", url);
                    return (true, imageUrl, null);
                }

                _logger.LogWarning("No image found for {Url}", url);
                return (false, null, "No image found on page");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error fetching {Url}", url);
                return (false, null, "Could not fetch webpage");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout fetching {Url}", url);
                return (false, null, "Request timed out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching {Url}", url);
                return (false, null, "Could not fetch webpage");
            }
        }
    }
}