using System.Reflection.Metadata;
using System.Text.Encodings.Web;
using HtmlAgilityPack;
using MyList.Scraper.Models;

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
            return [];
        }

        /*  
            Scrape "The List" and capture all show data  
        */
        public async Task<List<Show>> ScrapeListAsync()
        {   
            List<Uri> urls = await GetWeeklyPageUrls();
            List<Show> shows = [];

            foreach (Uri url in urls)
            {
                _logger.LogInformation(url.ToString());
            }

            return shows;
        }
    }
}