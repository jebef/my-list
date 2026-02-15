using MyList.Scraper;
using MyList.Scraper.Services;

var builder = Host.CreateApplicationBuilder(args);

//--- DI ---//
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient<ListScraper>();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000");
});

var host = builder.Build();
host.Run();
