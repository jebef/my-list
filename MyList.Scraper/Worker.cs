using MyList.Data;
using MyList.Scraper.Services;
namespace MyList.Scraper;

public class Worker(ILogger<Worker> logger, ListScraper scraper, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var shows = await scraper.ScrapeListAsync();

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Shows.AddRange(shows);
        await db.SaveChangesAsync(stoppingToken);
        logger.LogInformation("Saved {Count} shows to database", shows.Count);
    }
}
