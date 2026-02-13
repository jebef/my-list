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

                    string meta = string.Join(" ", show.SelectNodes("./text()")
                        .Select(n => n.InnerText.Trim())
                        .Where(text => text != "" && text != ","))
                        .Trim();

                    var ageMatch = Regex.Match(meta, @"(a/a|21\+|18\+|16\+|14\+|12\+)(\s+\(([^)]+)\))?");
                    if (ageMatch.Success)
                    {
                        s.AllAges = ageMatch.Groups[1].Value == "a/a";
                        s.AgeMeta = ageMatch.Groups[3].Success ? ageMatch.Groups[3].Value : null;
                    }
                    else
                    {
                        s.AllAges = false;
                    }

                    s.Recommended = meta.Contains(" *");
                    s.U21DrinkTix = meta.Contains(" ^");
                    s.PitWarning = meta.Contains(" @");
                    s.NoInsOuts = meta.Contains(" #");


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