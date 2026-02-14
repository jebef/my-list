using Microsoft.EntityFrameworkCore;
using MyList.Data;
using MyList.Scraper;
using MyList.Scraper.Services;

var builder = Host.CreateApplicationBuilder(args);

//--- Worker ---//
builder.Services.AddHostedService<Worker>();

//--- Database ---//
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//--- ListScraper ---// 
builder.Services.AddHttpClient<ListScraper>();

var host = builder.Build();
host.Run();
