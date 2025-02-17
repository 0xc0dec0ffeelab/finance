using notification;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddHostedService<EmailWorker>();

var host = builder.Build();
host.Run();
