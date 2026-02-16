using MyList.Notifier.Services;

namespace MyList.Notifier;

public class Worker(
    ILogger<Worker> logger,
    ShowFilter showFilter,
    EmailSender emailSender) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var shows = await showFilter.GetMyShowsAsync();
        logger.LogInformation("Found {Count} shows that match your preferences", shows.Count);

        if (shows.Count > 0)
        {
            await emailSender.SendAsync(shows);
        }
        else
        {
            logger.LogInformation("No matching shows found, no email sent");
        }
    }
}
