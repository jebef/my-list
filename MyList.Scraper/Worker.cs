using MyList.Scraper.Services;
namespace MyList.Scraper;

public class Worker(ILogger<Worker> logger, ListScraper scraper) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var shows = await scraper.ScrapeListAsync();
        logger.LogInformation("Scraped {Count} shows from The List", shows.Count);
    }
}
