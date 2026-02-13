using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Linq;
using MyList.Scraper.Models;
using System.Net;
using Microsoft.VisualBasic;

namespace MyList.Scraper.Services
{
    public class ListScraper(HttpClient httpClient, ILogger<ListScraper> logger)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<ListScraper> _logger = logger;
        private readonly Uri _baseUrl = new Uri("http://www.foopee.com/punk/the-list/");

        /* 
            Get the HTML contents of a given URL 
        */
        public async Task<string> GetHtml(Uri url)
        {
            try
            {
                string html = await _httpClient.GetStringAsync(url);
                return html;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"\nCaught HTTP Request Exception: {e.Message}");
                return string.Empty;
            }
        }

        /*
            Fetch all "Concert By Date" URLs

            Each page contains all shows for that week
        */
        public async Task<List<Uri>> GetWeeklyPageUrls()
        {
            List<Uri> weeklyUrls = [];

            // capture main page html  
            string html = await GetHtml(_baseUrl);
            if (String.IsNullOrEmpty(html))
            {
                _logger.LogError("\nFailed to fetch weekly page URLs");
                return [];
            }

            // load html into doc
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // parse weekly links 
            var nodes = doc.DocumentNode.SelectNodes("//dl//a");
            foreach (var node in nodes)
            {
                string href = node.GetAttributeValue("href", "");
                string text = node.InnerText;
                if (String.IsNullOrEmpty(href)) continue;
                if (!href.StartsWith("by-date")) break;
                var weeklyUrl = new Uri(_baseUrl, href);
                weeklyUrls.Add(weeklyUrl);
            }

            return weeklyUrls;
        }

        /* 
            Capture show data for each page/week 
        */
        public List<Show> ParseWeeklyPage(string html)
        {
            List<Show> weeklyShows = [];

            // load html into doc
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // parse shows for this week 
            var nodes = doc.DocumentNode.SelectNodes("//body/ul/li");
            foreach (var node in nodes)
            {
                // capture day and shows 
                string day = node.SelectSingleNode(".//a//b").InnerText; // "Wed Feb 11"
                var shows = node.SelectNodes(".//ul//li");

                // parse shows for this day 
                foreach (var show in shows)
                {
                    string[] location = show.SelectSingleNode("./b/a").InnerText.Split(",");

                    List<string> artists = show.SelectNodes("./a")
                        .Select(artist => artist.InnerText)
                        .ToList();

                    var s = new Show
                    {
                        Date = DateOnly.ParseExact(day, "ddd MMM d", null),
                        Venue = location[0].Trim(),
                        City = location[1].Trim(),
                        Artists = artists
                    };

                    // capture and parse metadata 
                    string meta = string.Join(" ", show.SelectNodes("./text()")
                        .Select(n => n.InnerText.Trim())
                        .Where(text => text != "" && text != ","))
                        .Trim();

                    // age 
                    var ageMatch = Regex.Match(meta, @"(a/a|21\+|18\+|16\+|14\+|12\+)(\s+\(([^)]+)\))?");
                    if (ageMatch.Success && ageMatch.Groups[1].Value == "a/a")
                    {
                        s.AllAges = true;
                        s.AgeMeta = ageMatch.Groups[3].Success ? ageMatch.Groups[3].Value : null;
                    }
                    else if (ageMatch.Success)
                    {
                        s.AllAges = false;
                        s.AgeMeta = ageMatch.Groups[3].Success ?
                            ageMatch.Groups[1].Value + ageMatch.Groups[3].Value : ageMatch.Groups[1].Value;
                    }
                    else
                    {
                        s.AllAges = false;
                        s.AgeMeta = null;
                    }

                    // price 
                    var priceMatch = Regex.Match(meta, @"\$(\d+(?:\.\d{2})?)");
                    if (priceMatch.Success && decimal.TryParse(priceMatch.Groups[1].Value, out decimal price))
                    {
                        s.Price = price;
                    }
                    if (meta.Contains("free")) s.Price = 0;

                    // time 
                    string[] timeFormats = ["htt", "hhtt", "h:mmtt", "hh:mmtt"];
                    // "and" format (5pm and 7:30pm)
                    var andStartTimesMatch = Regex.Match(meta, @"\s(\d{1,2}(?::\d{2})?(?:am|pm))\s(?:and)\s(\d{1,2}(?::\d{2})?(?:am|pm))");
                    if (andStartTimesMatch.Success)
                    {
                        TimeOnly.TryParseExact(andStartTimesMatch.Groups[1].Value, timeFormats, null,
                            System.Globalization.DateTimeStyles.None, out TimeOnly t1);
                        TimeOnly.TryParseExact(andStartTimesMatch.Groups[2].Value, timeFormats, null,
                            System.Globalization.DateTimeStyles.None, out TimeOnly t2);
                        s.StartTimes = [t1, t2];
                    }
                    else
                    {
                        // "doors/show" format (8pm/8:30pm) or single time (8pm)
                        var timesMatch = Regex.Matches(meta, @"\d{1,2}(?::\d{2})?(?:am|pm)");
                        if (timesMatch.Count >= 2)
                        {
                            TimeOnly.TryParseExact(timesMatch[0].Value, timeFormats, null,
                                System.Globalization.DateTimeStyles.None, out TimeOnly doors);
                            TimeOnly.TryParseExact(timesMatch[1].Value, timeFormats, null,
                                System.Globalization.DateTimeStyles.None, out TimeOnly start);
                            s.DoorsTime = doors;
                            s.StartTimes = [start];
                        }
                        else if (timesMatch.Count == 1)
                        {
                            TimeOnly.TryParseExact(timesMatch[0].Value, timeFormats, null,
                                System.Globalization.DateTimeStyles.None, out TimeOnly start);
                            s.StartTimes = [start];
                        }
                    }

                    // symbols 
                    s.Recommended = meta.Contains(" *");
                    s.U21DrinkTix = meta.Contains(" ^");
                    s.PitWarning = meta.Contains(" @");
                    s.NoInsOuts = meta.Contains(" #");
                    var willSellOutMatch = Regex.Match(meta, @"\$[^0-9]");
                    s.WillSellOut = willSellOutMatch.Success;

                    weeklyShows.Add(s);
                    _logger.LogInformation(s.ToString());
                }

            }

            return weeklyShows;
        }

        /*
            Scrape "The List" and capture all show data
        */
        public async Task<List<Show>> ScrapeListAsync()
        {
            List<Uri> urls = await GetWeeklyPageUrls();
            List<Show> shows = [];

            int counterDebug = 0;

            foreach (Uri url in urls)
            {
                if (counterDebug > 0) break;
                _logger.LogInformation(url.ToString());
                string html = await GetHtml(url);
                ParseWeeklyPage(html);
                counterDebug++;
            }

            return shows;
        }
    }
}