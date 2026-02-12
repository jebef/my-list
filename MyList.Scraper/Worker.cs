using MyList.Scraper.Services;
namespace MyList.Scraper;

public class Worker(ILogger<Worker> logger, ListScraper scraper) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await scraper.ScrapeListAsync();
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     if (logger.IsEnabled(LogLevel.Information))
        //     {
        //         logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     }
        //     await Task.Delay(1000, stoppingToken);
        // }
    }
}
