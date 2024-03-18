using ClickHouse.Client.ADO;
using Hangfire;
using Hangfire.Redis.StackExchange;
using StackExchange.Redis;
using Uptimed.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add Jobs processing 
await using var connection = new ClickHouseConnection(builder.Configuration.GetConnectionString("UptimedLogs")!);
builder.Services.AddTransient<ClickHouseConnection>();

var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("UptimedJobs")!);
GlobalConfiguration.Configuration.UseRedisStorage(redis)
    .UseActivator(new ContainerJobActivator(builder.Services.BuildServiceProvider()));

using var server = new BackgroundJobServer();
Console.WriteLine("Hangfire Server started. Press any key to exit...");


var host = builder.Build();
host.Run();