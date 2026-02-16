using MyList.Notifier;
using MyList.Notifier.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient<ShowFilter>(client =>
{
   client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5185");
});

builder.Services.AddSingleton<EmailSender>();

var host = builder.Build();
host.Run();
