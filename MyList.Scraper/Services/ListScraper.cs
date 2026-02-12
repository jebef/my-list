using System.Text.Encodings.Web;
using MyList.Scraper.Models;

namespace MyList.Scraper.Services
{
    public class ListScraper(HttpClient httpClient, ILogger<ListScraper> logger)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<ListScraper> _logger = logger;

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
        public List<Uri> GetWeeklyPageUrls()
        {
            return [];
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
            Uri testUrl = new Uri("http://www.foopee.com/punk/the-list/by-date.0.html");
            List<Uri> urls = [testUrl];
            List<Show> shows = [];

            foreach (Uri url in urls)
            {
                string html = await GetHtml(url);
                _logger.LogInformation(html);
            }

            return shows;
        }
    }
}