using System.Net.Http.Json;
using MyList.Scraper.Services;

namespace MyList.Scraper;

public class Worker(
    ILogger<Worker> logger, 
    ListScraper scraper,
    IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var shows = await scraper.ScrapeListAsync();
        logger.LogInformation("Scraped {Count} shows from The List", shows.Count);

        var client = httpClientFactory.CreateClient("ApiClient");
        var resp = await client.PostAsJsonAsync("/api/shows", shows, stoppingToken);

        if (resp.IsSuccessStatusCode)
        {
            var res = await resp.Content.ReadAsStringAsync();
            logger.LogInformation("API Response: {Result}", res);
        }
        else
        {
            logger.LogError("Failed to post shows: {Status}", resp.StatusCode);
        }
    }
}
