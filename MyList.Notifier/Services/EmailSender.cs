using MailKit.Net.Smtp;
using MimeKit;
using MyList.Shared.Models;

namespace MyList.Notifier.Services;

public class EmailSender(ILogger<EmailSender> logger, IConfiguration config)
{
    /* 
        Send a list of curated shows via email 
    */
    public async Task SendAsync(List<Show> shows)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MyList", "noreply@mylist.com"));
        message.To.Add(new MailboxAddress("", "wyattjebef@gmail.com")); 
        message.Subject = $"{shows.Count} upcoming shows this week";

        message.Body = new TextPart("plain")
        {
            Text = FormatShowList(shows)
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            config["Smtp:Host"] ?? "smtp.gmail.com",
            int.Parse(config["Smtp:Port"] ?? "587"),
            MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(config["Smtp:Username"], config["Smtp:Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        logger.LogInformation("Email sent with {Count} shows", shows.Count);
    }

    /* 
        Helper to format email contents 
    */
    private string FormatShowList(List<Show> shows)
    {
        var lines = new List<string> { "Here are your upcoming shows:\n" };

        foreach (var show in shows)
        {
            lines.Add($"{show.Date} @ {show.Venue}, {show.City}");
            lines.Add($"  Artists: {string.Join(", ", show.Artists)}");
            if (show.Price is not null) lines.Add($"  Price: ${show.Price}");
            if (show.DoorsTime is not null) lines.Add($"  Doors: {show.DoorsTime}");
            lines.Add("");
        }

        return string.Join("\n", lines);
    }
}
