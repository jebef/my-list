using MyList.Notifier;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient<ShowFilter>(client =>
{
   client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5185");
});

var host = builder.Build();
host.Run();
