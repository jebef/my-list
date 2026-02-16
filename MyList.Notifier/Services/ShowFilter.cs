using System.Net.Http.Json;
using MyList.Shared.Models;

public class ShowFilter (HttpClient httpClient, ILogger<ShowFilter> logger)
{
    // HARDCODED DEV FILTERS 
    private List<string> cities = ["S.F."]; 
    private decimal maxPrice = 30;
    private DateOnly nextWeek = DateOnly.FromDateTime(DateTime.Now.AddDays(7));

    public async Task<List<Show>> GetMyShowsAsync()
    {
        try
        {
            var res = await httpClient.GetAsync("/api/shows");
            res.EnsureSuccessStatusCode();

            var shows = await res.Content.ReadFromJsonAsync<List<Show>>();
            if (shows is null) throw new HttpRequestException("JSON parsing error");

            return shows.Where(s => 
                s.Date >= DateOnly.FromDateTime(DateTime.Now) && 
                s.Date <= nextWeek && 
                (s.Price is null || s.Price <= maxPrice) && 
                cities.Contains(s.City))
                .ToList();
        }
        catch (HttpRequestException e)
        {
            logger.LogError("Failed to fetch show data from API: {Message}", e.Message);
            return [];
        }
    }
}