using MyList.Scraper;
using MyList.Scraper.Services;

var builder = Host.CreateApplicationBuilder(args);

//--- Worker ---//
builder.Services.AddHostedService<Worker>();

//--- ListScraper ---// 
builder.Services.AddHttpClient<ListScraper>();

var host = builder.Build();
host.Run();
